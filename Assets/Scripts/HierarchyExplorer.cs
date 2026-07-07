using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно добавляем для работы со сценой

public class NN_HierarchyExporter : MonoBehaviour
{
    [Tooltip("Имя файла, который будет создан в папке Assets")]
    public string fileName = "FullHierarchyLog.txt";

    void Start()
    {
        ExportEntireSceneForNN();
    }

    void ExportEntireSceneForNN()
    {
        StringBuilder sb = new StringBuilder();
        
        // Получаем имя текущей активной сцены
        Scene currentScene = SceneManager.GetActiveScene();
        
        sb.AppendLine($"--- ПОЛНАЯ ИЕРАРХИЯ СЦЕНЫ: {currentScene.name} ---");
        sb.AppendLine("Формат: Полный/Путь/До/Объекта -> [Компонент1, Компонент2]");
        sb.AppendLine("---------------------------------------------------------");

        // Получаем ВСЕ корневые (верхнеуровневые) объекты на сцене
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject rootObj in rootObjects)
        {
            // Запускаем рекурсивный сбор для каждого корневого дерева
            BuildTree(rootObj.transform, "", sb);
        }

        // Формируем путь для сохранения файла в папку Assets вашего проекта
        string filePath = Path.Combine(Application.dataPath, fileName);
        
        // Записываем весь собранный текст в файл
        File.WriteAllText(filePath, sb.ToString());
        
        Debug.Log($"[УСПЕХ] Полный лог всей сцены сохранен! Файл: {filePath}");
    }

    void BuildTree(Transform current, string parentPath, StringBuilder sb)
    {
        // Формируем точный путь от корня сцены
        string currentPath = string.IsNullOrEmpty(parentPath) ? current.name : parentPath + "/" + current.name;
        
        // Получаем все компоненты на текущем объекте
        Component[] components = current.GetComponents<Component>();
        string compList = "";
        
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] != null) // Защита от "Missing" компонентов
            {
                compList += components[i].GetType().Name;
                if (i < components.Length - 1) compList += ", ";
            }
        }

        // Добавляем строку в лог
        sb.AppendLine($"{currentPath} -> [{compList}]");

        // Рекурсивно идем по всем дочерним объектам
        foreach (Transform child in current)
        {
            BuildTree(child, currentPath, sb);
        }
    }
}