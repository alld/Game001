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
    * �߾� 5,5 ������ ����Ʈ �÷��̾� ����, Ư���� ��� ���� ��Ż
    * �÷��̾������� ��� ��ġ�� ���� ����, ������ ���� ��ġ��
    * 
    * 10�� ���� ��ġ�� �й��ؼ� �ڿ�, Ư�� ���� ��ġ��
    * 
    * 10�� �ֿ� ���� ��ġ�� ���¿����� �ֺ��� ���� �����Ŵ
    * 
    * ������ �ֿ� ������ �����ϰ� ���ʿ� ������� �ش� ������ �ֿ��������� ����
    * ������ �ֿ������� 3���̻� ����������� �Ŵ������� ����
    * �Ŵ����������� Ư���� ����� ������. 
    * 
    * �������� ���Ǿ������� ȣ��
    * �ܺα��� ����Ǿ������� �ٴ�
    * ���� ���� ����Ǿ������� ������ �з��� 
    * 
    * ������ ������ ������, ȭ������, �縷����, �ʿ�, ��, ��, ���
    * 
    * ����, ������, NPC����, 
    *
    *
    *
    */
}
