using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Events;
using Interfaces;
using Model;
using UnityEngine;
using YandexGames;

namespace Core.Platform
{
    /// <summary>
    /// Реализация сервиса сохранения и загрузки для платформы Yandex Games.
    /// </summary>
    public class YandexSaveLoadService : ISaveLoadService
    {
        private readonly GridModel _gridModel;
        private readonly StatisticsModel _statisticsModel;
        private readonly ActionCountersModel _actionCountersModel;

        private bool _isLoading;
        private bool _isSaving;

        /// <summary>
        /// Инициализирует сервис сохранения/загрузки с необходимыми моделями.
        /// </summary>
        public YandexSaveLoadService(GridModel gridModel, StatisticsModel statisticsModel, ActionCountersModel actionCountersModel)
        {
            _gridModel = gridModel;
            _statisticsModel = statisticsModel;
            _actionCountersModel = actionCountersModel;
        }

        /// <summary>
        /// Запрашивает сохранение игры.
        /// </summary>
        public void RequestSave()
        {
            SaveGame();
        }

        /// <summary>
        /// Сохраняет игру, используя новый компактный формат для сетки.
        /// </summary>
        private void SaveGame()
        {
            if (_isLoading || _isSaving) return;
            _isSaving = true;
            var gridStringBuilder = new StringBuilder();
            var grid = _gridModel.Cells;
            for (var lineIndex = 0; lineIndex < grid.Count; lineIndex++)
            {
                var line = grid[lineIndex];
                var lineParts = (from cell in line where cell.IsActive select $"{cell.Column}:{cell.Number}").ToList();
                if (lineParts.Count > 0)
                {
                    gridStringBuilder.Append(string.Join(",", lineParts));
                }

                if (lineIndex < grid.Count - 1)
                {
                    gridStringBuilder.Append('|');
                }
            }

            YandexGamesSdk.Saves.gridState = gridStringBuilder.ToString();

#pragma warning disable 0618
            if (YandexGamesSdk.Saves.gridCells != null)
            {
                YandexGamesSdk.Saves.gridCells.Clear();
            }
#pragma warning restore 0618
            YandexGamesSdk.Saves.statistics = new StatisticsModelSerializable(_statisticsModel);
            YandexGamesSdk.Saves.actionCounters = new ActionCountersModelSerializable(_actionCountersModel);
            try
            {
                var jsonSaves = JsonUtility.ToJson(YandexGamesSdk.Saves);
                var sizeInBytes = Encoding.UTF8.GetByteCount(jsonSaves);
                var sizeInKilobytes = sizeInBytes / 1024f;
                Debug.Log($"Размер сохранения: {sizeInKilobytes:F2} КБ. Строка сетки: {YandexGamesSdk.Saves.gridState.Length} символов.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при расчете размера сохранения: {e.Message}");
            }

            Debug.Log("Запрос на сохранение игровых данных через YandexSaveLoadService.");
            YandexGamesSdk.SaveProgress();
            _isSaving = false;
        }


        /// <summary>
        /// Загружает игру с серверов Yandex, поддерживая старый и новый форматы.
        /// </summary>
        /// <param name="onComplete">Callback, вызываемый по завершении. True при успехе.</param>
        public void LoadGame(Action<bool> onComplete)
        {
            _isLoading = true;
            var savedCells = new List<CellDataSerializable>();
#pragma warning disable 0618
            if (YandexGamesSdk.Saves.gridCells != null && YandexGamesSdk.Saves.gridCells.Count > 0)
            {
                Debug.Log("Обнаружены старые данные сохранения (gridCells). Производится миграция на новый формат.");
                savedCells = YandexGamesSdk.Saves.gridCells;
            }
#pragma warning restore 0618
            else if (!string.IsNullOrEmpty(YandexGamesSdk.Saves.gridState))
            {
                Debug.Log("Загрузка из нового компактного формата (gridState).");
                var lines = YandexGamesSdk.Saves.gridState.Split('|');
                for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                {
                    var lineData = lines[lineIndex];
                    if (string.IsNullOrEmpty(lineData)) continue;
                    var cellParts = lineData.Split(',');
                    foreach (var part in cellParts)
                    {
                        try
                        {
                            var pair = part.Split(':');
                            if (pair.Length != 2) continue;
                            var column = int.Parse(pair[0]);
                            var number = int.Parse(pair[1]);
                            savedCells.Add(new CellDataSerializable
                            {
                                number = number,
                                line = lineIndex,
                                column = column,
                                isActive = true
                            });
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Ошибка парсинга данных ячейки '{part}': {e.Message}");
                        }
                    }
                }
            }

            _gridModel.RestoreState(savedCells);
            _statisticsModel.SetState(YandexGamesSdk.Saves.statistics.score, YandexGamesSdk.Saves.statistics.multiplier);
            _actionCountersModel.RestoreState(YandexGamesSdk.Saves.actionCounters);
            
            GlobalEvents.OnToggleTopLine?.Invoke(YandexGamesSdk.Saves.isTopLineVisible);
            GlobalEvents.OnStatisticsChanged?.Invoke((YandexGamesSdk.Saves.statistics.score, YandexGamesSdk.Saves.statistics.multiplier));

            Debug.Log("Игровые данные успешно загружены из сохранений Yandex.");
            _isLoading = false;
            onComplete?.Invoke(true);
        }

        /// <summary>
        /// Устанавливает видимость верхней строки и сохраняет состояние.
        /// </summary>
        public void SetTopLineVisibility(bool isVisible)
        {
            YandexGamesSdk.Saves.isTopLineVisible = isVisible;
        }
    }
}