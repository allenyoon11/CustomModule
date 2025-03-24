using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class GraphModel
{
    public enum OVERFLOW_TYPE
    {
        TRUNCATE,
        FLOW,
    }

    public enum AXIS_TYPE
    {
        X,
        Y,
    }

    public enum LINE_TYPE
    {
        SOLID,
        DASHED,
    }

    public Vector2ReactiveProperty RangeXRx = new(new Vector2(0, 10f)); // (xMin, xMax)
    public Vector2ReactiveProperty RangeYRx = new(new Vector2(-40f, 40f)); // (yMin, yMax)
    public ReactiveCollection<GraphLineModel> ListLineModelsRx = new();
    public ReactiveCollection<GraphLegendModel> ListLegendModelRx = new(); //INFO::legend Rx; by allen
    public ReactiveCollection<GraphDotModel> ListDotModelRx = new(); //INFO::dot; by allen
    public ReactiveProperty<GraphAxis> AxisXRx = new(); //INFO::axis info for axix name...; by allen
    public ReactiveProperty<GraphAxis> AxisYRx = new(); //INFO::axis info for axix name...; by allen
    
    public Subject<(GraphLineModel lineModel, bool active)> OnLineActiveChanged { get; private set; } = new Subject<(GraphLineModel, bool)>(); //INFO::Active line; by allen
    public Subject<GraphLegendModel> OnLegendToggleChanged { get; private set; } = new Subject<GraphLegendModel>(); //INFO::Active lines by child legend toggle change; by allen

    public string Name => name; //INFO::check graph model; by allen
    public float RangeX => RangeXRx.Value.y - RangeXRx.Value.x;
    public float RangeY => RangeYRx.Value.y - RangeYRx.Value.x;

    public Queue<float> XTicks => xTicks;
    public Queue<float> YTicks => yTicks;
    public float XTickInterval => xTickInterval; //INFO::for xTick text format; by allen
    public float YTickInterval => yTickInterval; //INFO::for yTick text format; by allen

    //public float[] NormalizedTicksX => xTicks.Select(tick => (tick - RangeXRx.Value.x) / (RangeXRx.Value.y - RangeXRx.Value.x)).ToArray();
    //public float[] NormalizedTicksY => xTicks.Select(tick => (tick - RangeYRx.Value.x) / (RangeYRx.Value.y - RangeYRx.Value.x)).ToArray();

    private const float scrollFactor = 0.1f;

    private string name;
    private Vector2 domainX;
    private Vector2 domainY;
    private Vector2 anchor;
    private Vector2 scale;
    private Vector2 pivot;
    private Vector2 defaultTickInterval;
    private float borderTickness;
    private float maxScale;
    
    private Queue<float> xTicks;
    private Queue<float> yTicks;
    private float xTickInterval;
    private float yTickInterval;

    //INFO::added by allen
    public OVERFLOW_TYPE overflowType; //COMMENT::view need to know overflow type; by allen
    public GraphAxis axisX;
    public GraphAxis axisY;
    public GraphModel(string name, Vector2 xDomain, Vector2 yDomain, Vector2 anchor, OVERFLOW_TYPE overflowType, Vector2 defaultTickInterval, Vector2 scale, Vector2 pivot, float maxScale, float borderThickness)
    {
        this.name = name;
        this.domainX = xDomain;
        this.domainY = yDomain;
        this.anchor = anchor;
        this.overflowType = overflowType;
        this.defaultTickInterval = defaultTickInterval;

        this.scale = scale;
        this.pivot = pivot;

        this.maxScale = maxScale;

        borderTickness = borderThickness;

        xTicks = new();
        yTicks = new();
        xTickInterval = defaultTickInterval.x; // by allen
        yTickInterval = defaultTickInterval.y; // by allen

        axisX = new GraphAxis(null, null, null, null, null, null, null,null, Color.black, Color.black, Color.black); //INFO::add axis info by allen
        axisY = new GraphAxis(null, null, "R", null, "L", null, null, null, Color.red, Color.blue, Color.black); //INFO::add axis info by allen
        Init();
    }

    public GraphModel(string name, Vector2 domainX, Vector2 domainY, Vector2 anchor, OVERFLOW_TYPE overflowType, Vector2 thickInterval)
        : this(name, domainX, domainY, anchor, overflowType, thickInterval, scale: Vector2.one, pivot: 0.5f * Vector2.one, maxScale: 30.0f, borderThickness: 0.25f) { }

    //INFO::update graphAxis
    public GraphModel(string name, Vector2 domainX, Vector2 domainY, Vector2 anchor, OVERFLOW_TYPE overflowType, Vector2 tickInterval, GraphAxis axisX, GraphAxis axisY)
    {

        this.name = name;
        this.domainX = domainX;
        this.domainY = domainY;
        this.anchor = anchor;
        this.overflowType = overflowType;
        this.defaultTickInterval = tickInterval;
        this.scale = Vector2.one;
        this.pivot = 0.5f * Vector2.one;
        this.maxScale = 30.0f;
        this.borderTickness = 0.25f;

        this.xTicks = new();
        this.yTicks = new();

        this.xTickInterval = defaultTickInterval.x; // by allen
        this.yTickInterval = defaultTickInterval.y; // by allen
        this.axisX = axisX; // by allen
        this.axisY = axisY; // by allen

        Init();
    }
    public void Init()
    {
        scale = Vector2.one;
        pivot = 0.5f * Vector2.one;
        SetRangeX();
        SetRangeY();
    }

    public void DragX(float pivotX)
    {
        float _tempPivotX = pivot.x - pivotX / scale.x;
        pivot.x = Mathf.Clamp(_tempPivotX, 0.5f / scale.x, 1f - (0.5f / scale.x));
        SetRangeX();
    }

    public void DragY(float pivotY)
    {
        float _tempPivotY = pivot.y - pivotY / scale.y;
        pivot.y = Mathf.Clamp(_tempPivotY, 0.5f / scale.y, 1f - (0.5f / scale.y));
        SetRangeY();
    }

    /// <summary>
    /// Scroll 로 인한 Scale, Pivot 의 변화를 계산
    /// </summary>
    /// <param name="scrollDeltaX">x축으로의 스크롤 입력값</param>
    /// <param name="scrollPivotX">현재 Graph View에서 스크롤 입력이 이루어진 지점의 x축 normalized point (-0.5 ~ 0.5)</param>
    public void ScrollX(float scrollDeltaX, float scrollPivotX)
    {
        // _scalePivotX : scale의 pivot, scrollPivot
        float _scalePivotX = pivot.x + scrollPivotX / scale.x;
        // 현재 Scale값에 임시로 증감값을 반영
        float _tempScaleX = scale.x;
        if (scrollDeltaX > 0)
            _tempScaleX += scrollFactor;
        else if (scrollDeltaX < 0)
            _tempScaleX -= scrollFactor;

        // 증감값이 적용된 임시 scale값을 Clamp하여 확정 반영
        scale.x = Mathf.Clamp(_tempScaleX, 1.0f, maxScale);
        // Scroll Pivot을 유지하며, 새 Scale 값을 반영했을 때의 Pivot값을 역으로 산출하여 임시로 저장
        float _tempPivotX = _scalePivotX - scrollPivotX / scale.x;
        // 임시 pivot 값을 Clamp하여 확정 반영
        pivot.x = Mathf.Clamp(_tempPivotX, 0.5f / scale.x, 1f - (0.5f / scale.x));
        // 확정된 scale, pivot 값으로 새 Range 산출
        SetRangeX();
    }

    public void ScrollY(float scrollDeltaY, float scrollPivotY)
    {
        float _scalePivotY = pivot.y + scrollPivotY / scale.y;
        float _tempScaleY = scale.y;
        if (scrollDeltaY > 0)
            _tempScaleY += scrollFactor;
        else if (scrollDeltaY < 0)
            _tempScaleY -= scrollFactor;

        scale.y = Mathf.Clamp(_tempScaleY, 1.0f, maxScale);

        float _tempPivotY = _scalePivotY - scrollPivotY / scale.y;
        pivot.y = Mathf.Clamp(_tempPivotY, 0.5f / scale.y, 1f - (0.5f / scale.y));
        SetRangeY();
    }

    public void SetDomainX(Vector2 domainX)
    {
        this.domainX = domainX;
        SetRangeX();
    }

    public void SetDomainY(Vector2 domainY)
    {
        this.domainY = domainY;
        SetRangeY();
    }

    public void SetRangeX()
    {
        var _start = Mathf.Lerp(domainX.x, domainX.y, pivot.x - 0.5f / scale.x);
        var _end = Mathf.Lerp(domainX.x, domainX.y, pivot.x + 0.5f / scale.x);

        Vector2 _rangeX = new Vector2(_start, _end);
        SetTick(_rangeX, ref xTicks, ref xTickInterval);
        RangeXRx.Value = _rangeX;
        AxisXRx.Value = axisX; //add by allen;
    }

    public void SetRangeY()
    {
        var _start = Mathf.Lerp(domainY.x, domainY.y, pivot.y - 0.5f / scale.y);
        var _end = Mathf.Lerp(domainY.x, domainY.y, pivot.y + 0.5f / scale.y);

        Vector2 _rangeY = new Vector2(_start, _end);
        SetTick(_rangeY, ref yTicks, ref yTickInterval);
        RangeYRx.Value = _rangeY;
        AxisYRx.Value = axisY; //add by allen;
    }

    /// <summary>
    /// 각 축의 range에 해당하는 tick을 계산하여 해당 축 queue에 저장
    /// </summary>
    /// <param name="range">해당 축의 range</param>
    /// <param name="ticks">해당 축의 tick을 담을 queue</param>
    /// <param name="tickInterval">tick 텍스트 자리수를 위하여 tickInterval 값을 public 으로 할당; by allen</param>
    public void SetTick(Vector2 range, ref Queue<float> ticks, ref float tickInterval)
    {
        Assert.IsNotNull(ticks);
        if (ticks == null) return; //by allen

        // x축 tick을 저장할 quque를 초기화
        ticks.Clear();

        // x축 range의 길이
        float _length = range.y - range.x;

        // 가수범위를 [1, 10) 으로 사용하기 위해, 상용로그를 취한 값에 -1 적용
        float _exp = Mathf.Ceil(Mathf.Log10(_length)) - 1;
        // 지수를 사용하여 [1, 10) 범위의 가수를 구함
        float _mantissa = _length / Mathf.Pow(10, _exp);

        // 기본 tick의 단위는 10의 지수배로 설정
        float _tickInterval = Mathf.Pow(10, _exp);
        // 범위에 따라 기본 tick단위에 계수를 조정
        // [1,2) = 0.2, [2,5) = 0.5, [5,10) = 1
        if (_mantissa < 2)
            _tickInterval *= 0.2f;
        else if (_mantissa < 5)
            _tickInterval *= 0.5f;

        // range.y는 항상 range.x보다 커야하며, tick interval은 항상 0보다 커야함.
        Assert.IsTrue(range.x < range.y);
        if (range.x >= range.y) return; //by allen
        Assert.IsTrue(_tickInterval > 0);
        if (_tickInterval <= 0) return; //by allen
                                        // min < 0 이고 max > 0 인경우
        if (range.x * range.y < 0)
        {
            // 0
            ticks.Enqueue(0);

            // 음수부분 계산
            float _tickValue = -_tickInterval;
            while (range.x <= _tickValue)
            {
                ticks.Enqueue(_tickValue);
                _tickValue -= _tickInterval;
            }
            // 양수부분 계산
            _tickValue = _tickInterval;
            while (_tickValue <= range.y)
            {
                ticks.Enqueue(_tickValue);
                _tickValue += _tickInterval;
            }
        }
        // min < 0, max < 0 이거나 min > 0, max > 0 인 경우
        else
        {
            if (range.x * range.y < float.Epsilon)
                ticks.Enqueue(0);

            // 첫 tick 계산
            float _tickValue = range.x - (range.x % _tickInterval) + _tickInterval;

            if (range.y < 0)
            {
                _tickValue = range.x - (range.x % _tickInterval);
            }

            while (_tickValue <= range.y)
            {
                ticks.Enqueue(_tickValue);
                _tickValue += _tickInterval;
            }
        }
        tickInterval = _tickInterval;
    }

    public Dictionary<string, GraphLineModel> dicGraphLine = new();
    public Dictionary<string, GraphLegendModel> dicGraphLegend = new();
    public Dictionary<string, GraphDotModel> dicGraphDot = new();
    public GraphDotModel AddGraphDot(string name, Color color, float radius, Vector2 pos)
    {
        var no = GetGraphDots(name).Count + 1;
        var key = $"{name}-{no}";
        bool hasDot = dicGraphLegend.ContainsKey(key);
        Assert.IsFalse(hasDot, $"{key} is exist already.");

        if (!hasDot)
        {
            var _graphDotModel = new GraphDotModel(no, name, color, radius, pos);
            dicGraphDot.Add(key, _graphDotModel);
            ListDotModelRx.Add(_graphDotModel);
            return _graphDotModel;
        }
        return null;
    }
    public void RemoveGraphDot(int no, string name)
    {
        var key = $"{name}-{no}";
        bool hasDot = dicGraphDot.TryGetValue(key, out GraphDotModel _graphDotModel);
        Assert.IsTrue(hasDot, $"{key} is not exist");
        if (hasDot)
        {
            dicGraphDot.Remove(key);
            ListDotModelRx.Remove(_graphDotModel);
        }
    }
    public List<GraphDotModel> GetGraphDots(string name, int no = -1)
    {
        return ListDotModelRx.Where(dot => no > 0 ? dot.LegendRx.Value == name && dot.NoRx.Value == no : dot.LegendRx.Value == name).ToList();
    }
    public List<GraphDotModel> GetGraphDots(Color color, int no = -1)
    {
        return ListDotModelRx.Where(dot => no > 0 ? dot.ColorRx.Value == color && dot.NoRx.Value == no : dot.ColorRx.Value == color).ToList();
    }
    public void ActiveGraphDots(bool active, string name, int no = -1)
    {
        List<GraphDotModel> dots = GetGraphDots(name, no);
        dots.ForEach(dot => dot.ActiveRx.Value = active);
    }
    public void ActiveGraphDots(bool active, Color color, int no = -1)
    {
        List<GraphDotModel> dots = GetGraphDots(color, no);
        dots.ForEach(dot => dot.ActiveRx.Value = active);
    }
    //INFO::Add Legend; by allen
    public GraphLegendModel AddGraphLegend(string name, Color color, bool isOn)
    {
        bool hasLegend = dicGraphLegend.ContainsKey(name);
        Assert.IsFalse(hasLegend, $"{name} is exist already.");

        if (!hasLegend)
        {
            var _graphLegendModel = new GraphLegendModel(name, color, isOn);
            dicGraphLegend.Add(name, _graphLegendModel);
            ListLegendModelRx.Add(_graphLegendModel);
            return _graphLegendModel;
        }
        return null;
    }
    //INFO::Remove Legend; by allen
    public void RemoveGraphLegend(string name)
    {
        bool hasLegend = dicGraphLegend.TryGetValue(name, out GraphLegendModel _graphLegendModel);
        Assert.IsTrue(hasLegend, $"{name} is not exist");
        if (hasLegend)
        {
            dicGraphLegend.Remove(name);
            ListLegendModelRx.Remove(_graphLegendModel);
        }
    }
    //INFO::Set Legend IsOn; by allen
    public void SetGraphLegendToggle(string name, bool isOn)
    {
        bool hasLegend = dicGraphLegend.TryGetValue(name, out GraphLegendModel _graphLegendModel);
        Assert.IsTrue(hasLegend, $"{name} is not exist");
        if (hasLegend)
        {
            _graphLegendModel.IsOnRx.Value = isOn;
            _graphLegendModel.OnToggleChanged.OnNext(isOn);
        }
    }
    // INFO:: Update the line according to its legend status; by Yang
    public bool IsLegendOn(string name)
    {
        bool hasLegend = dicGraphLegend.TryGetValue(name, out GraphLegendModel _graphLegendModel);
        Assert.IsTrue(hasLegend, $"{name} is not exist");

        return _graphLegendModel.IsOnRx.Value;

    }

    public GraphLineModel AddGraphLine(string name, Color color, float thickness)
    {
        bool hasLine = dicGraphLine.ContainsKey(name); //for build; by allen
        Assert.IsFalse(hasLine, $"{name} is exist already."); //add message; by allen

        var _graphLineModel = new GraphLineModel(name, color, thickness);
        dicGraphLine.Add(name, _graphLineModel);
        ListLineModelsRx.Add(_graphLineModel);
        return _graphLineModel;
    }

    public void RemoveGraphLine(string name)
    {
        bool hasLine = dicGraphLine.TryGetValue(name, out GraphLineModel _graphLineModel); //for build; by allen
        Assert.IsTrue(hasLine, $"{name} is not exist"); //add message; by allen

        dicGraphLine.Remove(name);
        ListLineModelsRx.Remove(_graphLineModel);
    }

    public GraphLineModel GetGraphLine(string name)
    {
        if (dicGraphLine.TryGetValue(name, out GraphLineModel _graphLineModel))
            return _graphLineModel;
        else
            return null;
    }
    //INFO::show/hide line
    public void ActiveGraphLine(string name, bool active)
    {
        bool hasLine = dicGraphLine.TryGetValue(name, out GraphLineModel _graphLineModel); //for build; by allen
        if (hasLine)
        {
            OnLineActiveChanged.OnNext((_graphLineModel, active));
        }
        else
        {
            //Debug.Log("line not exist");
        }

    }
    public void AddValue(string name, Vector2 value, Color? color = null)
    {
        bool hasLine = dicGraphLine.TryGetValue(name, out GraphLineModel _graphLineModel); //for build; by allen
        Assert.IsTrue(hasLine, $"{name} is not exist"); //add message; by allen


        Color _color = color ?? _graphLineModel.ColorRx.Value;

        var _listData = _graphLineModel.ListLineData;

        Vector2 _outputValue = value;

        if (overflowType == OVERFLOW_TYPE.FLOW)
        {
            if (_outputValue.x > domainX.y)
            {
                float _offset = Mathf.Max(_outputValue.x - domainX.y);
                SetDomainX(new Vector2(domainX.x + _offset, domainX.y + _offset));
                _listData.RemoveAll(polypoint => polypoint.point.x < domainX.x);
            }
        }
        else
        {
            if (_outputValue.x > domainX.y)
                _outputValue.x = _outputValue.x % domainX.magnitude;
            if (_outputValue.x < _graphLineModel.LastData.x)
                _listData.Clear();
            _graphLineModel.LastData = _outputValue;
        }

        _listData.Add(new Shapes.PolylinePoint(_outputValue, _color));

        _graphLineModel.IsChangedThisFrame = true;

        //_graphLineModel.NotifyLineChange();
    }

    public void ClearAllGraphLine()
    {
        var lineNameList = new List<string>();

        foreach (var item in dicGraphLine)
        {
            lineNameList.Add(item.Key);
        }

        foreach (var lineName in lineNameList)
        {
            RemoveGraphLine(lineName);
        }

        dicGraphLine.Clear();
        Init();
    }
}
