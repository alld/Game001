using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_MapManager : MonoBehaviour
{
    public bool _editorSetting;

    public Transform[,] transforms;

    private const int MapSizeX = 10;
    private const int MapSizeY = 10;

    public void CreateFiled()
    {
        
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                Instantiate(transform);
            }
        }
    }
    

    

    /*
    * 중앙 5,5 지점은 디폴트 플레이어 지점, 특수한 경우 지점 이탈
    * 플레이어지점은 모든 배치가 끝난 직후, 그위에 변경 배치됨
    * 
    * 10의 랜덤 수치를 분배해서 자원, 특수 공간 배치됨
    * 
    * 10의 주요 지형 배치된 형태에따라 주변의 값을 변경시킴
    * 
    * 동일한 주요 지형이 인접하게 양쪽에 있을경우 해당 지형도 주요지형으로 변경
    * 동일한 주요지형이 3개이상 묶여있을경우 거대지구로 변경
    * 거대지구에서는 특별한 대상이 존재함. 
    * 
    * 물지형은 고립되어있으면 호수
    * 외부까지 연결되어있으면 바다
    * 물이 서로 연결되어있으면 강으로 분류함 
    * 
    * 지형의 종류는 늪지대, 화산지대, 사막지대, 초원, 물, 숲, 평원
    * 
    * 던전, 적기지, NPC마을, 
    *
    *
    *
    */
}
