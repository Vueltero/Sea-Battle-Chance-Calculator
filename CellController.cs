using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CellType _cellType;
    [SerializeField] private Sprite _waterSprite, _shipSprite, _missSprite, _hitSprite;

    private CellState _cellState;
    public CellState CellState => _cellState;

    private bool _hasShip;
    private int _posX, _posY;

    public void Init(int id, bool hasShip)
    {
        ResetCell();

        _posX = id % 10;
        _posY = id / 10;

        _hasShip = hasShip;
    }

    private void ResetCell()
    {
        _cellState = CellState.CAN_FIRE;
        _hasShip = false;
        _text.text = "";
        _text.color = Color.white;
        _image.sprite = _waterSprite;
    }

    public void CellClick()
    {
        if (_cellType == CellType.SET_UP)
        {
            _image.sprite = _cellState == CellState.WATER ? _shipSprite : _waterSprite;
            _cellState = _cellState == CellState.WATER ? CellState.SHIP : CellState.WATER;
        }
        else //cell in chance grid
        {
            if (_cellState == CellState.CAN_FIRE)
            {
                _cellState = _hasShip ? CellState.HIT : CellState.MISS;
                _image.sprite = _cellState switch
                {
                    CellState.MISS => _missSprite,
                    CellState.HIT => _hitSprite
                };

                GameManager.Instance.SetCellState(_posX, _posY, _cellState);
            }
        }
    }

    public void SetText(double chance)
    {
        chance *= 100;

        _text.text = $"{chance:F2}%";
    }

    public void SetTextColor(Color color)
    {
        _text.color = color;
    }
}

[Serializable]
public enum CellState : byte
{
    WATER,
    SHIP,

    CAN_FIRE,
    MISS,
    HIT
}

[Serializable]
public enum CellType : byte
{
    SET_UP,
    CHANCE
}
