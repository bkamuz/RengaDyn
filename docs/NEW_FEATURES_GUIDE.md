# 🎯 Руководство по новым возможностям RengaDyn 2.0

## 🚀 Краткий обзор новинок

После рефакторинга RengaDyn получил множество новых возможностей, сохранив полную обратную совместимость. Это руководство покажет, как использовать новые функции для улучшения ваших Dynamo скриптов.

---

## 🏭 GeometryFactory - Новый способ работы с геометрией

### ✨ Создание точек и векторов

```csharp
// СТАРЫЙ способ (всё ещё работает)
var oldPoint = Base.SetPoint3D(1.0, 2.0, 3.0);

// НОВЫЙ способ - типобезопасный
var newPoint = Base.CreatePoint3D(1.0, 2.0, 3.0);           // Point3D
var floatPoint = Base.CreateFloatPoint3D(1.0f, 2.0f, 3.0f); // FloatPoint3D

// Прямое использование фабрики
var point2D = GeometryFactory.CreatePoint2D<Point2D>(10, 20);
var vector3D = GeometryFactory.CreateVector3D<Vector3D>(1, 0, 0);
```

### 🔍 Универсальное извлечение координат

```csharp
// СТАРЫЙ способ - нужно знать тип
var coords = Base.GetCoords_2D(geometry, 1); // 1 = Point2D

// НОВЫЙ способ - универсально
var coords2D = Base.ExtractCoords2D(anyGeometry2D);
var coords3D = Base.ExtractCoords3D(anyGeometry3D);
// Работает с Point2D, FloatPoint2D, Vector2D автоматически!
```

---

## 🎨 Улучшенная работа со стилями

### 📊 Получение информации о категориях

```csharp
// Получить все доступные категории оборудования
var equipmentCategories = EquipmentStyle.GetEquipmentCategories();
// Результат: {"Other": EquipmentCategory_Other, "Faucet": EquipmentCategory_Faucet, ...}

// Получить категории сантехники
var plumbingCategories = PlumbingFixtureStyle.GetPlumbingFixtureCategories();

// Получить типы арматурных элементов
var reinforcementTypes = ReinforcementUnitStyle.GetReinforcementUnitTypes();
```

### 🔧 Расширенная отладочная информация

```csharp
// Любой стиль теперь предоставляет детальную отладку
var style = new BeamStyle(comObject);
var debugInfo = style.GetDebugInfo();

Console.WriteLine(debugInfo);
/* Вывод:
🔧 Wrapper Type: BeamStyle
📋 Interface Type: IBeamStyle
✅ Is Valid: True
🆔 Interface Hash: A1B2C3D4
📝 Style ID: 42
🏷️ Style Name: Standard Beam
*/
```

---

## 🏗️ Универсальное создание объектов модели

### 🎯 Новый метод CreateObjectByType

```csharp
// СТАРЫЙ способ - отдельные методы для каждого типа
var wall = model.CreateWall(levelId, placement3D);
var column = model.CreateColumn(levelId, placement3D);

// НОВЫЙ способ - универсальный метод с отладкой
var result = model.CreateObjectByType("wall", levelId, placement3D, enableDebug: true);
var wall = result["ModelObject"];
var debugInfo = result["DebugInfo"];

// Поддерживаются все типы: "wall", "column", "door", "window", "floor", "level", "room", "equipment", "beam"
```

### 📋 Получение типов объектов

```csharp
// Получить все доступные типы объектов Renga
var objectTypes = Model.GetAvailableObjectTypes();
var wallGuid = Model.GetObjectTypeGuid("Wall");
```

---

## 🔧 Улучшенная работа с параметрами

### 🛡️ Безопасное получение значений

```csharp
var parameter = new Parameter(comObject);

// СТАРЫЙ способ - может выдать исключение
var oldValue = parameter.GetIntValue(); // Может упасть если нет значения

// НОВЫЙ способ - безопасный
var safeValue = parameter.GetIntValue(); // Вернёт -1 если нет значения
var hasValue = parameter.HasValue;       // Проверить наличие значения

// Универсальное получение значения
var anyValue = parameter.GetValue(); // Автоматически определит тип
```

### 📊 Справочная информация

```csharp
// Получить все типы значений параметров
var valueTypes = Parameter.GetParameterValueTypes();

// Получить тип параметра как строку
var typeString = parameter.GetValueTypeAsString();
// Результат: "ParameterValueType_String"
```

---

## 🔍 Отладка и диагностика

### 🕵️ Универсальная отладочная информация

```csharp
// Любой wrapper объект теперь поддерживает отладку
var model = new Model(project);
var debugInfo = model.GetDebugInfo();

var style = new SystemStyle(comObject);  
var styleDebug = style.GetDebugInfo();

var parameter = new Parameter(comObject);
var paramDebug = parameter.GetDebugInfo();
```

### ✅ Проверка валидности

```csharp
// Любой wrapper может проверить свою валидность
if (model.IsValid())
{
    // Безопасно использовать
}

if (style.IsValid())
{
    var name = style.Name; // Не выдаст исключение
}
```

---

## 🔄 ID конвертация - новые методы

### 🆔 Универсальные методы конвертации

```csharp
// СТАРЫЕ методы (устарели, но работают)
var id1 = model.GetIdFromUniqueId(guid);
var id2 = model.GetIdFromUniqueIdS(guidString);

// НОВЫЕ методы - универсальные
var id3 = model.ConvertUniqueIdToId(guid);        // Guid -> int
var id4 = model.ConvertUniqueIdToId(guidString);  // string -> int

var guid1 = model.ConvertIdToUniqueId(id);           // int -> Guid
var guid2 = model.ConvertIdToUniqueId(id, true);     // int -> string
```

---

## ⚡ Практические примеры

### 🏗️ Создание пола с отладкой

```csharp
// Получить доступные уровни
var levels = model.GetAvailableLevels();
var levelId = levels.First();

// Создать размещение
var placement = Placement3D.ByOrigin(Point.ByCoordinates(0, 0, 0));

// Создать пол с отладочной информацией
var result = model.CreateFloor(levelId, placement, enableDebug: true);
var floor = result["Floor"];
var debugInfo = result["DebugInfo"];

Console.WriteLine(debugInfo);
// Покажет весь процесс создания с детальной информацией
```

### 🎨 Работа с категориями оборудования

```csharp
// Получить все категории
var categories = EquipmentStyle.GetEquipmentCategories();

// Создать оборудование разных категорий
foreach(var category in categories)
{
    var categoryId = (int)category.Value;
    var equipment = model.CreateEquipment(levelId, categoryId, placement);
    Console.WriteLine($"Создано оборудование: {category.Key}");
}
```

### 🔧 Безопасная работа с параметрами

```csharp
// Получить параметры объекта
var parameters = modelObject.GetParameters();

foreach(var param in parameters)
{
    if (param.HasValue)
    {
        var value = param.GetValue();        // Универсально
        var type = param.GetValueTypeAsString();
        
        Console.WriteLine($"Параметр: {type} = {value}");
        
        // Безопасная типизированная работа
        switch(param.ValueType)
        {
            case ParameterValueType.ParameterValueType_String:
                var stringValue = param.GetStringValue(); // Безопасно
                break;
            case ParameterValueType.ParameterValueType_Double:
                var doubleValue = param.GetDoubleValue(); // Безопасно
                break;
        }
    }
}
```

---

## 🚀 Рекомендации по использованию

### ✅ DO - Рекомендуется
- Используйте новые методы `CreateXXX` вместо `SetXXX`
- Применяйте `.GetDebugInfo()` для отладки проблем
- Используйте `CreateObjectByType()` для универсального создания
- Проверяйте `.IsValid()` перед использованием wrapper'ов
- Используйте статические методы для получения справочной информации

### ⚠️ AVOID - Избегайте
- Не используйте устаревшие методы в новом коде (они помечены `[Obsolete]`)
- Не игнорируйте отладочную информацию при проблемах
- Не забывайте проверять `HasValue` у параметров

---

## 📚 Дополнительные ресурсы

- **Полный отчет по рефакторингу**: `REFACTORING_SUMMARY.md`
- **API документация**: Смотрите комментарии в коде
- **Примеры использования**: Папка `/dyn/` с обновленными примерами

---

**Удачного использования новых возможностей RengaDyn 2.0!** 🎉