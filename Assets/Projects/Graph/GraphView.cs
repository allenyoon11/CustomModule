using Shapes;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

[RequireComponent(typeof(ObservableEventTrigger))]
public class GraphView : MonoBehaviour, IInitializable
{
    public ObservableEventTrigger EventTrigger => eventTrigger;
    public RectTransform RectTransform => rectTransform;
    public Camera Cam { get; private set; } = null;

    private const float tickMarginX = 10.0f;
    private const float tickMarginY = 10.0f;

    [SerializeField]
    private Polyline rectangleBorder;
    [SerializeField]
    private Transform tickContainer;
    [SerializeField]
    private Transform gridContainer;

    [SerializeField]
    private ObservableEventTrigger eventTrigger;
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField] GraphAxisView axisView; //add by allen;


    private Color gridColor = new Color(0, 0, 0, 0.2f);
    private Color axisColor = new Color(0, 0, 0, 0.5f);

    private List<GraphGrid> listGridX;
    private List<GraphGrid> listGridY;

    private List<GraphTick> listTickX;
    private List<GraphTick> listTickY;

    private List<GraphLineView> listLine;

    private Dictionary<GraphLineModel, GraphLineView> dicLine = new();
    private Dictionary<GraphLegendModel, GraphLegendView> dicLegend = new();
    private Dictionary<GraphDotModel, GraphDotView> dicDot = new();


    //[Inject]
    //private VirtualDataGenerator generator;

    //private Queue<Vector2> graphDatas;

    private float[] arrNormGridX;
    private float[] arrNormGridY;

    private GraphGrid.Pool poolGraphGrid;
    private GraphTick.Pool poolGraphTick;
    private GraphLineView.Factory factoryGraphLine;
    private GraphLegendView.Factory factoryGraphLegend;
    private GraphDotView.Factory factoryGraphDot;


    [Inject]
    public void Construct(
        GraphGrid.Pool poolGraphGrid,
        GraphTick.Pool poolGraphTick,
        GraphLineView.Factory factoryGraphLine,
        GraphLegendView.Factory factoryGraphLegend,
        GraphDotView.Factory factoryGraphDot
        )
    {
        this.poolGraphGrid = poolGraphGrid;
        this.poolGraphTick = poolGraphTick;
        this.factoryGraphLine = factoryGraphLine;
        this.factoryGraphLegend = factoryGraphLegend;
        this.factoryGraphDot = factoryGraphDot;
    }
    private void Awake()
    {
        //INFO::Set UI Camera for graph event; by allen
        if (Cam == null) Cam = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
    }
    public void Initialize()
    {
        InitTick();
        InitGrid();
    }
    public void InitTick()
    {
        // Init Tick Pool
        InitPool(poolGraphTick, ref listTickX);
        InitPool(poolGraphTick, ref listTickY);
    }
    public void InitGrid()
    {
        // Init Grid Pool
        InitPool(poolGraphGrid, ref listGridX);
        InitPool(poolGraphGrid, ref listGridY);
    }
    //INFO::added tickInterval; by allen
    public void ResetX(Vector2 rangeX, in Queue<float> tickValues, float tickInterval)
    {
        float _width = rectTransform.rect.width;
        float _height = rectTransform.rect.height;


        float[] _arrTickValues = tickValues.ToArray();
        float[] _arrNormTickPositions = _arrTickValues.Select(tick => (tick - rangeX.x) / (rangeX.y - rangeX.x)).ToArray();

        float _normZeroX = (0 - rangeX.x) / (rangeX.magnitude);

        //SetAxisX(_width, _height, _normZeroX);

        SetGridX(_width, _height, _arrTickValues, _arrNormTickPositions);
        SetTickX(_width, _height, _arrTickValues, _arrNormTickPositions, tickInterval);
    }

    //INFO::create method because of frame drop; set grid and tick when graph overflowType is truncate only; added tickInterval; by allen
    public void ResetX(Vector2 rangeX, in Queue<float> tickValues, float tickInterval, GraphAxis axisX, GraphModel.OVERFLOW_TYPE overflowType)
    {
        float _width = rectTransform.rect.width;
        float _height = rectTransform.rect.height;


        float[] _arrTickValues = tickValues.ToArray();
        float[] _arrNormTickPositions = _arrTickValues.Select(tick => (tick - rangeX.x) / (rangeX.y - rangeX.x)).ToArray();

        float _normZeroX = (0 - rangeX.x) / (rangeX.magnitude);

        //SetAxisX(_width, _height, _normZeroX);

        if (overflowType == GraphModel.OVERFLOW_TYPE.TRUNCATE)
        {
            SetGridX(_width, _height, _arrTickValues, _arrNormTickPositions);
            SetTickX(_width, _height, _arrTickValues, _arrNormTickPositions, tickInterval, axisX);
        }
    }

    //INFO::added tickinterval
    public void ResetY(Vector2 rangeY, in Queue<float> tickValues, float tickInterval, GraphAxis axisY)
    {
        float _width = rectTransform.rect.width;
        float _height = rectTransform.rect.height;

        float[] _arrTickValues = tickValues.ToArray();
        float[] _arrNormTickPositions = _arrTickValues.Select(tick => (tick - rangeY.x) / (rangeY.y - rangeY.x)).ToArray();

        float _normZeroY = (0 - rangeY.x) / (rangeY.magnitude);

        //SetAxisY(_width, _height, _normZeroY);
        SetGridY(_width, _height, _arrTickValues, _arrNormTickPositions);
        SetTickY(_width, _height, _arrTickValues, _arrNormTickPositions, tickInterval, axisY);
    }

    public void ResetBorder()
    {
        float _width = RectTransform.rect.width;
        float _height = RectTransform.rect.height;
        rectangleBorder.SetPointPosition(0, new Vector3(0, 0, 0));
        rectangleBorder.SetPointPosition(1, new Vector3(_width, 0, 0));
        rectangleBorder.SetPointPosition(2, new Vector3(_width, _height, 0));
        rectangleBorder.SetPointPosition(3, new Vector3(0, _height, 0));
    }

    public void SetGridX(float width, float height, in float[] arrTickValues, in float[] arrNormGridPositions)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormGridPositions.Length);

        ReassignListItemsFromPool(arrNormGridPositions.Length, poolGraphGrid, listGridX);

        Assert.IsTrue(listGridX.Count == arrNormGridPositions.Length);

        for (int i = 0; i < listGridX.Count; i++)
        {
            var _grid = listGridX[i];

            float _positionX = width * arrNormGridPositions[i];

            //_grid.IsDashed = Mathf.Abs(arrTickValues[i]) > float.Epsilon;
            if (Mathf.Abs(arrTickValues[i]) < float.Epsilon)
                _grid.Color = axisColor;
            else
                _grid.Color = gridColor;

            _grid.Start = new Vector2(_positionX, 0);
            _grid.End = new Vector2(_positionX, height);
        }
    }


    public void SetGridY(float width, float height, in float[] arrTickValues, in float[] arrNormGridPositions)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormGridPositions.Length);

        ReassignListItemsFromPool(arrNormGridPositions.Length, poolGraphGrid, listGridY);

        Assert.IsTrue(listGridY.Count == arrNormGridPositions.Length);

        for (int i = 0; i < listGridY.Count; i++)
        {
            var _grid = listGridY[i];
            float _positionY = height * arrNormGridPositions[i];

            //_grid.IsDashed = Mathf.Abs(arrTickValues[i]) > float.Epsilon;
            if (Mathf.Abs(arrTickValues[i]) < float.Epsilon)
                _grid.Color = axisColor;
            else
                _grid.Color = gridColor;

            _grid.Start = new Vector2(0, _positionY);
            _grid.End = new Vector2(width, _positionY);
        }
    }
    public void SetTickX(float width, float height, in float[] arrTickValues, in float[] arrNormTickPositions, float tickInterval)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormTickPositions.Length);

        ReassignListItemsFromPool(arrNormTickPositions.Length, poolGraphTick, listTickX);

        Assert.IsTrue(listTickX.Count == arrNormTickPositions.Length);
        string format = "F0";
        if (tickInterval >= 2f) format = "F0";
        else if (tickInterval >= 0.1f) format = "F1";
        else if (tickInterval >= 0.001) format = "F2";
        else format = "F3";

        for (int i = 0; i < listTickX.Count; i++)
        {
            var _tick = listTickX[i];
            float _positionX = width * arrNormTickPositions[i];
            _tick.SetLocalPosition(new Vector2(_positionX, -tickMarginY));
            _tick.HorizontalAlign = TMPro.HorizontalAlignmentOptions.Center;
            _tick.VerticalAlgin = TMPro.VerticalAlignmentOptions.Top;
            _tick.Pivot = new Vector2(0.5f, 1);
            _tick.SetPreFix(null);
            _tick.SetColor(Color.black);
            _tick.SetValue(arrTickValues[i], format:format); //added format; by allen
        }
    }
    //added by allen;
    public void SetTickX(float width, float height, in float[] arrTickValues, in float[] arrNormTickPositions, float tickInterval, GraphAxis axis)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormTickPositions.Length);

        ReassignListItemsFromPool(arrNormTickPositions.Length, poolGraphTick, listTickX);

        Assert.IsTrue(listTickX.Count == arrNormTickPositions.Length);
        string format = "F0";
        if (tickInterval >= 2f) format = "F0";
        else if (tickInterval >= 0.1f) format = "F1";
        else if (tickInterval >= 0.001) format = "F2";
        else format = "F3";

        for (int i = 0; i < listTickX.Count; i++)
        {
            var _tick = listTickX[i];
            float _positionX = width * arrNormTickPositions[i];
            _tick.SetLocalPosition(new Vector2(_positionX, -tickMarginY));
            _tick.HorizontalAlign = TMPro.HorizontalAlignmentOptions.Center;
            _tick.VerticalAlgin = TMPro.VerticalAlignmentOptions.Top;
            _tick.Pivot = new Vector2(0.5f, 1);
            float _value = arrTickValues[i];
            if (_value > 0)
            {
                _tick.SetPreFix(axis.positivePrefix);
                _tick.SetPostFix(axis.positivePostfix);
                _tick.SetColor(axis.positiveColor);
            }
            else if (_value < 0)
            {
                _tick.SetPreFix(axis.negativePrefix);
                _tick.SetPostFix(axis.negativePostfix);
                _tick.SetColor(axis.negativeColor);
            }
            else
            {
                _tick.SetPreFix(axis.normalPrefix);
                _tick.SetPostFix(axis.normalPostfix);
                _tick.SetColor(axis.normalColor);
            }
            _tick.SetValue(arrTickValues[i], format: format); //added format; by allen
        }
    }

    public void SetTickY(float width, float height, in float[] arrTickValues, in float[] arrNormTickPositions, float tickInterval)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormTickPositions.Length);

        ReassignListItemsFromPool(arrNormTickPositions.Length, poolGraphTick, listTickY);

        Assert.IsTrue(listTickY.Count == arrNormTickPositions.Length);

        string format = "F0";
        if (tickInterval >= 2f) format = "F0";
        else if (tickInterval >= 0.1f) format = "F1";
        else if (tickInterval >= 0.001) format = "F2";
        else format = "F3";

        for (int i = 0; i < listTickY.Count; i++)
        {
            var _tick = listTickY[i];
            float _positionY = height * arrNormTickPositions[i];
            _tick.SetLocalPosition(new Vector2(-tickMarginX, _positionY));
            _tick.HorizontalAlign = TMPro.HorizontalAlignmentOptions.Right;
            _tick.VerticalAlgin = TMPro.VerticalAlignmentOptions.Middle;
            _tick.Pivot = new Vector2(1, 0.5f);
            float _value = arrTickValues[i];
            if (_value > 0)
            {
                _tick.SetPreFix("R");
                _tick.SetColor(Color.red);
            }
            else if (_value < 0)
            {
                _tick.SetPreFix("L");
                _tick.SetColor(Color.blue);
            }
            else
            {
                _tick.SetPreFix(null);
                _tick.SetColor(Color.black);
            }
            _tick.SetValue(Mathf.Abs(_value), format:format);
        }
    }
    //added by allen
    public void SetTickY(float width, float height, in float[] arrTickValues, in float[] arrNormTickPositions, float tickInterval, GraphAxis axis)
    {
        Assert.IsTrue(arrTickValues.Length == arrNormTickPositions.Length);

        ReassignListItemsFromPool(arrNormTickPositions.Length, poolGraphTick, listTickY);

        Assert.IsTrue(listTickY.Count == arrNormTickPositions.Length);

        string format = "F0";
        if (tickInterval >= 2f) format = "F0";
        else if (tickInterval >= 0.1f) format = "F1";
        else if (tickInterval >= 0.001) format = "F2";
        else format = "F3";

        for (int i = 0; i < listTickY.Count; i++)
        {
            var _tick = listTickY[i];
            float _positionY = height * arrNormTickPositions[i];
            _tick.SetLocalPosition(new Vector2(-tickMarginX, _positionY));
            _tick.HorizontalAlign = TMPro.HorizontalAlignmentOptions.Right;
            _tick.VerticalAlgin = TMPro.VerticalAlignmentOptions.Middle;
            _tick.Pivot = new Vector2(1, 0.5f);
            float _value = arrTickValues[i];
            if (_value > 0)
            {
                _tick.SetPreFix(axis.positivePrefix);
                _tick.SetPostFix(axis.positivePostfix);
                _tick.SetColor(axis.positiveColor);
            }
            else if (_value < 0)
            {
                _tick.SetPreFix(axis.negativePrefix);
                _tick.SetPostFix(axis.negativePostfix);
                _tick.SetColor(axis.negativeColor);
            }
            else
            {
                _tick.SetPreFix(axis.normalPrefix);
                _tick.SetPostFix(axis.normalPostfix);
                _tick.SetColor(axis.normalColor);
            }
            _tick.SetValue(Mathf.Abs(_value), format: format);
        }
    }
    public void AddDot(GraphDotModel dotModel)
    {
        bool hasDot = dicDot.ContainsKey(dotModel);
        Assert.IsFalse(hasDot);
        var _dotView = factoryGraphDot.Create(dotModel);
        _dotView.name = $"{dotModel.LegendRx.Value}-{dotModel.NoRx.Value}";
        dicDot.Add(dotModel, _dotView);
    }
    public void RemoveDot(GraphDotModel dotModel)
    {
        bool hasDot = dicDot.TryGetValue(dotModel, out GraphDotView _dotView);
        Assert.IsTrue(hasDot);

        Destroy(_dotView.gameObject);
        dicDot.Remove(dotModel);
    }
    public void AddLegendView(GraphLegendModel legendModel)
    {
        bool hasLegend = dicLegend.ContainsKey(legendModel);
        Assert.IsFalse(hasLegend);

        var _legendView = factoryGraphLegend.Create(legendModel);
        _legendView.name = legendModel.LegendRx.Value;
        dicLegend.Add(legendModel, _legendView);
    }
    public void RemoveLegendView(GraphLegendModel legendModel)
    {
        bool hasLegend = dicLegend.ContainsKey(legendModel);
        Assert.IsTrue(hasLegend);

        dicLegend.TryGetValue(legendModel, out GraphLegendView _legendModelView);
        Destroy(_legendModelView.gameObject);
        dicLegend.Remove(legendModel);
    }
    //public void SetLegendView(GraphLegendModel legendModel, bool isOn, bool setView = false)
    //{
    //	bool hasLegend = dicLegend.TryGetValue(legendModel, out GraphLegendView legendView);
    //	Assert.IsTrue(hasLegend);

    //	if (setView)
    //	{
    //		legendView.SetIsOn(isOn);
    //	}
    //   }
    public void SetAxisXInfo(GraphAxis axis)
    {
        axisView.SetAxisXName(axis.name, axis.unit, axis.normalColor);
    }
    public void SetAxisYInfo(GraphAxis axis)
    {
        axisView.SetAxisYName(axis.name, axis.unit, axis.normalColor);
    }
    public void AddLineView(GraphLineModel lineModel)
    {
        Assert.IsFalse(dicLine.ContainsKey(lineModel));

        bool active = true;
        if (dicLegend.Count > 0)
        {
            var targetLegend = dicLegend.Where(el => el.Key.ColorRx.Value == lineModel.ColorRx.Value).First();
            if (targetLegend.Key != null)
            {
                active = targetLegend.Key.IsOnRx.Value;
            }
        }
        var _lineModelView = factoryGraphLine.Create(lineModel);
        _lineModelView.gameObject.SetActive(active);
        _lineModelView.name = lineModel.LegendRx.Value; //INFO::set object name; by allen
        _lineModelView.SetThickness(lineModel.ThicknessRx.Value); //INFO::thickness
        dicLine.Add(lineModel, _lineModelView);
    }


    public void RemoveLineView(GraphLineModel lineModel)
    {
        Assert.IsTrue(dicLine.ContainsKey(lineModel));

        dicLine.TryGetValue(lineModel, out GraphLineView _lineModelView);
        Destroy(_lineModelView.gameObject);
        dicLine.Remove(lineModel);
    }
    //INFO::Active Line(show/hide); by allen
    public void ActiveLineView(GraphLineModel lineModel, bool active)
    {
        var exist = dicLine.TryGetValue(lineModel, out GraphLineView _lineModelView);
        Assert.IsTrue(exist);

        _lineModelView.gameObject.SetActive(active);
    }
    private void InitPool<T>(in MemoryPool<T> pool, ref List<T> list)
    {
        if (list == null)
        {
            list = new();
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                var _item = list[list.Count - 1 - i];
                pool.Despawn(_item);
                list.Remove(_item);
            }

        }
    }
    private void ReassignListItemsFromPool<T>(int length, in MemoryPool<T> pool, in List<T> list)
    {
        if (list.Count < length)
        {
            while (list.Count < length)
            {
                var _item = pool.Spawn();
                list.Add(_item);
            }
        }
        else
        {
            while (list.Count > length)
            {
                var _item = list.Last();
                pool.Despawn(_item);
                list.Remove(_item);
            }
        }
    }

    public class Factory : PlaceholderFactory<GraphModel, GraphView> { }
}
