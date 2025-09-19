# Rusleo Voxel Pathfinding

Воксельная система поиска пути для Unity: **LOD-сетка → граф → поиск пути (A\*) → сглаживание (Warp, Splines)**.
В комплекте есть **Gizmos-визуализация** и демо-сцена с агентом (пчела).

![Pathfinding Demo](Documentation~/Media/Preview.gif)

---

## 📦 Возможности

* **LOD-сетка** — рекурсивное деление занятых вокселей, соседи по 6 направлениям.
* **Граф** — узлы из свободных вокселей, рёбра между соседями.
* **Поиск пути (A\*)** — быстрый и детерминированный.
* **Warp & Splines** — сглаживание маршрута по граням и сплайнами.
* **Демо** — `BeeMover` с шумовым движением и ограничением ускорения.
* **Gizmos** — визуализация сетки, графа и путей.

---

## 🚀 Установка

Через Unity Package Manager → *Add package from git URL...*:

```
https://github.com/razrabVkedah/voxel-pathfinding.git
```

---

## 🔑 Использование

```csharp
var path = pathBuilder.BuildPath(start.position, end.position);
// path.PathNodes — основной путь
// pathBuilder.GetWarpedPath() — сглаженный вариант
```

---

## 📊 Демонстрация

![LOD Grid Example](Documentation~/Media/Demo_0.png)
![LOD Grid Example](Documentation~/Media/Demo_1.png)
![LOD Grid Example](Documentation~/Media/Demo_2.png)
![LOD Grid Example](Documentation~/Media/Demo_3.png)
![LOD Grid Example](Documentation~/Media/Demo_4.png)

---

## 🛣 Roadmap

* Более точный выбор достижимой стартовой точки (учёт препятствий).
* Оптимизация warp-маршрутов (без захода в центр вокселей).
* Допил сплайнов (Bezier, Catmull-Rom).

---

✦ Автор: [Rusleo](https://github.com/razrabVkedah)
