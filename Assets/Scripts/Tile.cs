using UnityEngine;
using System;
using TMPro;

namespace MineSweeper
{
    public class Tile : MonoBehaviour, ITile
    {
        public event Action<Tile> OnTileDigged;

        public TileType TileType => tileType;
        [SerializeField] private TileType tileType;
        [SerializeField] private Renderer renderer;
        [SerializeField] private TMP_Text numberText;

        private bool _wasFlaged;
        private bool _wasDigged;

        private void Awake()
        {
            numberText.text = string.Empty;
            numberText.gameObject.SetActive(false);
        }

        public void ResetTile(TileType tileType)
        {
            renderer.material.color = Color.white;
            numberText.text = string.Empty;
            numberText.gameObject.SetActive(false);
            this.tileType = tileType;
            _wasFlaged = false;
            _wasDigged = false;
        }

        public void SetTileText(string text) => numberText.text = text;

        public bool Open(bool checkFlag = false, bool invokeEvent = true)
        {
            if (checkFlag && _wasFlaged)
                return false;

            if (_wasDigged)
                return false;

            numberText.gameObject.SetActive(true);
            _wasDigged = true;

            if (invokeEvent)
                OnTileDigged?.Invoke(this);

            renderer.material.color = GetTileColor();

            return true;
        }

        public bool Dig() => Open(true);

        public bool Flag(out bool placed, out bool removed)
        {
            placed = false;
            removed = false;

            if (_wasDigged)
                return false;

            if (_wasFlaged)
            {
                removed = true;
                _wasFlaged = false;
            }
            else
            {
                placed = true;
                _wasFlaged = true;
            }
            renderer.material.color = _wasFlaged ? Color.yellow : Color.white;

            return true;
        }

        private Color GetTileColor()
        {
            switch (tileType)
            {
                case TileType.Empty:
                    return Color.blue;

                case TileType.Mine:
                    return Color.red;

                default:
                    return Color.white;
            }
        }
    }
}
