# ColorPickerUI

<p align="center">
<a href="https://www.nuget.org/packages/ColorPickerUI">
    <img src="https://img.shields.io/nuget/v/ColorPickerUI?logo=nuget&label=NuGet" alt="NuGet" />
</a>
<img title="" src="https://img.shields.io/badge/.Net-8.0%20Windows-blue?logo=dotnet" alt="Static Badge"/>
<img src="https://img.shields.io/badge/License-MIT-green?logo=opensourceinitiative" title="" alt="Static Badge" />
</p>
<p align="center">
      <img src="https://i.ibb.co/fGSZyFRg/Color-Picker-UI-Transparent.png" alt="Project Logo" width="150" />
</p>
<p align="center">
    <img src="https://i.ibb.co/G4pwM6NH/app-hsv.png" alt="Project Screenshot" width="150" />
    <img src="https://i.ibb.co/0PsvNFZ/app-rgb.png" alt="Project Screenshot" width="150" />
</p>
A customizable color picker control for WPF applications.

[English](#english) | [Русский](#русский)

---

# English

**ColorPickerUI** is a WPF color picker library that provides an intuitive interface for selecting and editing colors.

It supports HSV and RGB color models, HEX input, alpha transparency, customizable preset color palettes, color copying and screen color picking.

---

## Features

- **HSV color model** — adjust Hue, Saturation, and Value.

- **RGB color model** — edit Red, Green, and Blue values.

- **Alpha channel** — control color transparency.

- **HEX input** — enter colors using hexadecimal values.
- **Color copying** — copy the selected color in HEX, RGB, orARGB format.
- **Screen eyedropper** — pick colors directly from anywhere on the screen using a magnifier with pixel grid.

- **Preset color palettes** — choose from predefined color collections.

- **Custom palettes** — provide your own collection of preset colors.

- **Preset shapes** — choose between square and circular color previews.
- **Preset sizes** — choose the size of preset color previews.
- **Color preview** — display the currently selected color.

- **Two-way data binding** — bind the selected color to your ViewModel.

- **XML documentation** — IntelliSense descriptions for public API members.

---

## Requirements

- Windows

- .NET 8.0 or later

- WPF application

---

## Installation

Install the package from NuGet:

```powershell
Install-Package ColorPickerUI
```

Or using the .NET CLI:

```bash
dotnet add package ColorPickerUI
```

---

## Getting Started

### 1. Add the namespace

In your XAML file:

```xml
xmlns:controls="clr-namespace:ColorPickerUI.Controls;assembly=ColorPickerUI"
```

or in your code-behind:

```csharp
using ColorPickerUI.Controls;
```

### 2. Add the ColorPicker control

```xml
<controls:ColorPicker Width="342" Height="665"
SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
```

### 3. Bind the selected color

In your ViewModel:

```csharp
using System.Windows.Media;
public class MainViewModel
{
    public Color SelectedColor { get; set; } = Colors.DodgerBlue;
}
```

The `SelectedColor` property supports two-way binding, allowing changes in the color picker to be reflected in your ViewModel.

---

## Customization

ColorPickerUI provides several properties for customizing the control.

| Property                   | Description                                            |
| -------------------------- | ------------------------------------------------------ |
| `SelectedColor`            | Gets or sets the currently selected color.             |
| `ShowPresetColors`         | Controls the visibility of preset colors.              |
| `ShowPreviewSelectedColor` | Controls the visibility of the selected color preview. |
| `Palette`                  | Selects the preset color palette.                      |
| `PresetColorShape`         | Sets the shape of preset color previews.               |
| `PresetColorSize`          | Sets the size of preset color previews.                |
| `PresetCustomColors`       | Provides a custom collection of preset colors.         |

### Available palettes

- `Classic`

- `Material`

- `Pastel`

- `Grayscale`

- `Extended`

- `Custom`

### Preset color shapes

- `Circle`

- `Square`
### Preset color sizes
- `Small`
- `Normal`
- `Large`

---

## Example

```xml
<controls:ColorPicker Width="342" Height="665"
SelectedColor="{Binding SelectedColor, Mode=TwoWay}"
ShowPresetColors="Visible"
ShowPreviewSelectedColor="Visible"
Palette="Material"
PresetColorShape="Circle"
PresetColorSize="Large" />
```
### Custom preset colors
Set up a custom collection of predefined colors using the PresetCustomColors property.
In your XAML file:
```xml
<controls:ColorPicker
    Width="342"
    Height="665"
    Palette="Custom"
    PresetColorShape="Circle">
    <controls:ColorPicker.PresetCustomColors>
        <Color>Red</Color>
        <Color>Orange</Color>
        <Color>Yellow</Color>
        <Color>Green</Color>
        <Color>Blue</Color>
        <Color>Purple</Color>
    </controls:ColorPicker.PresetCustomColors>
</controls:ColorPicker>
```

or in your code-behind:

```csharp
using System.Collections.ObjectModel;
using System.Windows.Media;
using ColorPickerUI.Controls;
using ColorPickerUI.Enums;
var picker = new ColorPicker();
picker.Palette = ColorPaletteType.Custom;
picker.PresetCustomColors = new ObservableCollection<Color>
{
    Colors.Red,
    Colors.Orange,
    Colors.Yellow,
    Colors.Green,
    Colors.Blue,
    Colors.Purple
};
```

---

## Demo Application

The repository includes a demo WPF application that demonstrates how to use and customize the ColorPicker control.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

# Русский
**ColorPickerUI** — библиотека с настраиваемым элементом управления для выбора цвета в WPF-приложениях.

Она предоставляет удобный интерфейс для выбора и редактирования цветов, поддерживает цветовые модели HSV и RGB, ввод HEX-кода, прозрачность, настраиваемые палитры готовых цветов, копирование цвета и выбор цвета непосредственно с экрана.

## Возможности
- **Модель HSV** — настройка Hue, Saturation и Value.
- **Модель RGB** — настройка компонентов Red, Green и Blue.
- **Альфа-канал** — управление прозрачностью цвета.
- **Ввод HEX** — ввод цвета в шестнадцатеричном формате.
- **Копирование цвета** — копирование выбранного цвета в форматах HEX, RGB и ARGB.
- **Экранная пипетка** — выбор цвета из любой области экрана с помощью увеличительной лупы и сетки пикселей.
- **Готовые палитры** — выбор из предустановленных наборов цветов.
- **Пользовательские палитры** — возможность задать собственную коллекцию цветов.
- **Форма образцов цветов** — квадратные или круглые элементы.
- **Размер образцов цветов** — малый, обычный или большой.
- **Предварительный просмотр** — отображение выбранного цвета.
- **Двусторонняя привязка данных** — поддержка Binding с ViewModel.
- **XML-документация** — описания публичного API для IntelliSense.
---
## Требования
- Windows
- .NET 8.0
- WPF-приложение
---
## Установка
Установить пакет можно через NuGet:
```powershell
Install-Package ColorPickerUI
```
Или через .NET CLI:
```bash
dotnet add package ColorPickerUI
```
---
## Начало работы
### 1. Добавьте пространство имён
В XAML-файле добавьте:
```xml
xmlns:controls="clr-namespace:ColorPickerUI.Controls;assembly=ColorPickerUI"
```

или в code-behind:

```csharp
using ColorPickerUI.Controls;
```
### 2. Добавьте элемент ColorPicker
```xml
<controls:ColorPicker Width="342" Height="665"
SelectedColor="{Binding SelectedColor, Mode=TwoWay}" />
```
### 3. Привяжите выбранный цвет
В ViewModel:
```csharp
using System.Windows.Media;
public class MainViewModel
{
    public Color SelectedColor { get; set; } = Colors.DodgerBlue;
}
```
Свойство `SelectedColor` поддерживает двустороннюю привязку данных. Изменения цвета в ColorPicker будут передаваться в ViewModel.

## Настройка

| Свойство                   | Описание                                                            |
| -------------------------- | ------------------------------------------------------------------- |
| `SelectedColor`            | Получает или задаёт выбранный цвет.                                 |
| `ShowPresetColors`         | Управляет видимостью готовых цветов.                                |
| `ShowPreviewSelectedColor` | Управляет отображением предварительного просмотра выбранного цвета. |
| `Palette`                  | Выбирает палитру готовых цветов.                                    |
| `PresetColorShape`         | Задаёт форму образцов цветов.                                       |
| `PresetColorSize`          | Задаёт размер образцов цветов.                                      |
| `PresetCustomColors`       | Позволяет задать собственную коллекцию цветов.                      |

### Доступные палитры
- `Classic`
- `Material`
- `Pastel`
- `Grayscale`
- `Extended`
- `Custom`
### Форма образцов цветов
- `Circle` — круглая.
- `Square` — квадратная.
### Размер образцов цветов
- `Small` — малый.
- `Normal` — обычный.
- `Large` — большой.
---
## Пример
```xml
<controls:ColorPicker Width="342" Height="665"
SelectedColor="{Binding SelectedColor, Mode=TwoWay}"
ShowPresetColors="Visible"
ShowPreviewSelectedColor="Visible"
Palette="Material"
PresetColorShape="Circle"
PresetColorSize="Large" />
```
Установить собственную коллекцию готовых цветов с помощью свойства PresetCustomColors.
В XAML-файле добавьте:
```xml
<controls:ColorPicker
    Width="342"
    Height="665"
    Palette="Custom"
    PresetColorShape="Circle">
    <controls:ColorPicker.PresetCustomColors>
        <Color>Red</Color>
        <Color>Orange</Color>
        <Color>Yellow</Color>
        <Color>Green</Color>
        <Color>Blue</Color>
        <Color>Purple</Color>
    </controls:ColorPicker.PresetCustomColors>
</controls:ColorPicker>
```

или в code-behind:

```csharp
using System.Collections.ObjectModel;
using System.Windows.Media;
using ColorPickerUI.Controls;
using ColorPickerUI.Enums;
var picker = new ColorPicker();
picker.Palette = ColorPaletteType.Custom;
picker.PresetCustomColors = new ObservableCollection<Color>
{
    Colors.Red,
    Colors.Orange,
    Colors.Yellow,
    Colors.Green,
    Colors.Blue,
    Colors.Purple
};
```
---
## Демонстрационное приложение
В репозитории находится демонстрационное WPF-приложение, показывающее использование и настройку элемента ColorPicker.

## Лицензия

Проект распространяется под лицензией [MIT License](LICENSE).


