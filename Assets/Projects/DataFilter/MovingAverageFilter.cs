using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class MovingAverageFilter
    {
        private int windowSize;
        private int repetitions;
        private List<float[]> dataBufferList;
        private List<float> sumList;
        private int bufferIdx;
        private bool isBufferFilled;

        public MovingAverageFilter(int windowSize, int repetitions)
        {
            this.windowSize = windowSize;
            this.repetitions = repetitions;
            this.dataBufferList = new List<float[]>();
            this.sumList = new List<float>();

            this.bufferIdx = 0;
            this.isBufferFilled = false;

            // 각 반복에 대해 합계를 0으로 초기화
            for (int i = 0; i < repetitions; i++)
            {
                dataBufferList.Add(new float[windowSize]);
                sumList.Add(0f);
            }
        }

        // 데이터를 추가하고 최종 이동 평균을 반환하는 함수
        public float Update(float inputData)
        {
            if (windowSize == 0 || repetitions == 0) return inputData;
            //Debug.Log($"inputData: {inputData}");

            // 각 반복(repetitions)에 대해 계산
            for (int rep = 0; rep < repetitions; rep++)
            {
                if (!isBufferFilled)
                {
                    // 버퍼가 꽉 차지 않았을 때
                    sumList[rep] += inputData; // 각 반복의 합계를 업데이트
                    dataBufferList[rep][bufferIdx] = inputData; // 데이터를 버퍼에 저장
                    inputData = bufferIdx == 0 ? inputData : sumList[rep] / (bufferIdx + 1);
                }
                else
                {
                    // 버퍼가 가득 찼을 때
                    sumList[rep] += inputData - dataBufferList[rep][bufferIdx]; // 기존 값 제거하고 새로운 값 추가
                    dataBufferList[rep][bufferIdx] = inputData; // 새로운 데이터를 버퍼에 저장
                    inputData = sumList[rep] / windowSize; // 현재 반복의 평균을 계산
                }
                //Debug.Log($"bufferIdx : {bufferIdx} / rep : {rep} / data {inputData} / sumList : {string.Join(", ", sumList)}");
            }

            // 버퍼 인덱스를 증가시키고, 버퍼가 가득 찼는지 체크
            bufferIdx = (bufferIdx + 1) % windowSize;
            if (!isBufferFilled && bufferIdx == 0)
            {
                isBufferFilled = true;
            }
            //Debug.Log($"bufferIdx : {bufferIdx} / sumList : {string.Join(", ", sumList)}");
            // 최종 이동 평균을 반환 (마지막 repetition의 결과)
            if (isBufferFilled)
            {
                return sumList.Last() / windowSize;
            }
            else
            {
                return sumList.Last() / (bufferIdx);
            }
        }
    }

}