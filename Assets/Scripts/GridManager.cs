using System;
using System.Collections.Generic;
using UnityEngine;

namespace MineSweeper
{
    public class GridManager : MonoBehaviour
    {
        const int TILES_IN_POOL = 2000;

        public event Action OnWin;
        public event Action OnLose;

        [Header("Prefab")]
        [SerializeField] private ParticleSystem explosion;
        [SerializeField] private Tile tilePrefab;

        [Header("Game Settings")]
        [SerializeField] private bool spawnMinesOnClick = true;
        private int _fieldSizeX;
        private int _fieldSizeY;
        private int _minesCount;

        private int FieldSize => _fieldSizeX * _fieldSizeY;
        private int BiggerSide => Mathf.Max(_fieldSizeX, _fieldSizeY);
        private int LowerSide => Mathf.Min(_fieldSizeX, _fieldSizeY);

        private Tile[] _tilesPool;
        private AudioManager _audioManager;
        private bool _areMinesGenerated = false;
        private int _openedTiles;

        private void Awake()
        {
            // Создание пула клеток для дальнейшего использования
            _tilesPool = new Tile[TILES_IN_POOL];

            for (int i = 0; i < TILES_IN_POOL; i++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.gameObject.SetActive(false);
                tile.name = $"Tile {i}";
                tile.OnTileDigged += HandleTileInteraction; // Подписываемся на нажатие кнопки (клетки)
                _tilesPool[i] = tile;
            }
        }

        private void Start() => _audioManager = AudioManager.Instance;

        private void OnDestroy()
        {
            foreach (var tile in _tilesPool)
            {
                tile.OnTileDigged -= HandleTileInteraction; // Обязательно нужно отписаться
            }
        }

        public void StartGame(int sizeX, int sizeY, int mines)
        {
            _fieldSizeX = sizeX;
            _fieldSizeY = sizeY;
            _minesCount = mines;
            _areMinesGenerated = false;
            _openedTiles = 0;
            PlaceTiles();
        }

        private void PlaceTiles()
        {
            for (int rows = 0; rows < BiggerSide; rows++)
            {
                for (int cols = 0; cols < LowerSide; cols++)
                {
                    int tileIndex = rows * LowerSide + cols;
                    Tile tile = _tilesPool[tileIndex];
                    tile.transform.position = new Vector3(rows - BiggerSide * 0.5f, 0, cols - LowerSide * 0.5f);
                    tile.ResetTile(TileType.Empty);
                    tile.gameObject.SetActive(true);
                }
            }

            if (spawnMinesOnClick) return;

            GenerateMines();
        }

        private void HandleTileInteraction(Tile tile)
        {
            _openedTiles++;

            if (FieldSize - _minesCount == _openedTiles)
            {
                OnWin?.Invoke();
                _audioManager.PlayWinSound();
                return;
            }

            switch (tile.TileType)
            {
                case TileType.Empty:
                    if (!_areMinesGenerated)
                        GenerateMines(GetTileID(tile));

                    int neighboutMines = GetMinesAround(tile);

                    if (neighboutMines <= 0)
                        OpenAllNeighbours(tile);
                    break;

                case TileType.Mine:
                    OnLose?.Invoke();
                    _audioManager.PlayBoomSound();

                    explosion.transform.position = tile.transform.position + Vector3.one * 0.5f;
                    explosion.Play();

                    for (int i = 0; i < FieldSize; i++)
                        _tilesPool[i].Open(false, false);
                    break;
            }
        }

        private void GenerateMines(int tileID = -1)
        {
            for (int i = 0; i < _minesCount;)
            {
                int cell = UnityEngine.Random.Range(0, FieldSize - 1);

                if (tileID > 0 && tileID == cell)
                    continue;

                if (_tilesPool[cell].TileType != TileType.Mine)
                {
                    _tilesPool[cell].ResetTile(TileType.Mine);
                    i++;
                }
            }
            _areMinesGenerated = true;

            for (int i = 0; i < FieldSize; i++)
            {
                if (_tilesPool[i].TileType == TileType.Mine)
                    continue;

                int mines = GetMinesAround(_tilesPool[i]);

                if (mines <= 0)
                    continue;

                _tilesPool[i].SetTileText(mines.ToString());
            }
        }

        private int GetTileID(Tile tile)
        {
            for (int rows = 0; rows < BiggerSide; rows++)
            {
                for (int cols = 0; cols < LowerSide; cols++)
                {
                    int tileIndex = rows * LowerSide + cols;

                    if (_tilesPool[tileIndex] == tile)
                        return tileIndex;
                }
            }
            return 0;
        }

        private int GetMinesAround(Tile tile)
        {
            GetNeighbours(tile, out var neighbours);
            int mines = 0;

            foreach (var neighbour in neighbours)
            {
                if (neighbour.TileType == TileType.Mine)
                    mines++;
            }

            return mines;
        }

        private void OpenAllNeighbours(Tile tile)
        {
            GetNeighbours(tile, out var neighbours);

            foreach (var neighbourTile in neighbours)
            {
                if (neighbourTile.TileType == TileType.Mine)
                    continue;

                neighbourTile.Open(false, true);
            }
        }

        private void GetNeighbours(Tile tile, out List<Tile> neighbours)
        {
            int thisIndex = GetTileID(tile);
            neighbours = new List<Tile>();

            for (int rows = -1; rows <= 1; rows++)
            {
                for (int cols = -1; cols <= 1; cols++)
                {
                    int neighbourTile = thisIndex + rows * LowerSide + cols;

                    // Костыль
                    if (neighbourTile == 0 && thisIndex == LowerSide - 1)
                        continue;

                    if (neighbourTile == thisIndex)
                        continue;

                    if (neighbourTile > FieldSize - 1 || neighbourTile < 0)
                        continue;

                    int curRow = (thisIndex + rows * LowerSide) / LowerSide;

                    if (curRow != neighbourTile / LowerSide)
                        continue;

                    neighbours.Add(_tilesPool[neighbourTile]);
                }
            }
        }
    }
}
