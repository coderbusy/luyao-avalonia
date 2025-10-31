# LuYao.Avalonia

[English](#english) | [中文](#中文)

---

## 中文

LuYao.Avalonia 是一个为 Avalonia UI 框架提供的实用控件和行为库。

### 📦 NuGet 包

| 包名 | 版本 | 描述 |
|------|------|------|
| [LuYao.Avalonia.Behaviors](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) | [![NuGet](https://img.shields.io/nuget/v/LuYao.Avalonia.Behaviors.svg)](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) | Avalonia 行为库 |
| [LuYao.Avalonia.Fonts.MiSans.Regular](https://www.nuget.org/packages/LuYao.Avalonia.Fonts.MiSans.Regular) | [![NuGet](https://img.shields.io/nuget/v/LuYao.Avalonia.Fonts.MiSans.Regular.svg)](https://www.nuget.org/packages/LuYao.Avalonia.Fonts.MiSans.Regular) | MiSans Regular 字体集成 |

### ✨ 特性

#### LuYao.Avalonia.Behaviors

提供一组实用的 Avalonia 行为：

- **ImageFromFileBehavior**: 从文件路径加载图像，支持最大宽度和高度限制
  - 自动处理图像解码和尺寸优化
  - 支持动态更新图像源

#### LuYao.Avalonia.Fonts.MiSans.Regular

提供 MiSans Regular 字体的即用集成：

- 一行代码即可配置 MiSans 字体
- 无需手动管理字体文件

#### LuYao.Avalonia.Controls

提供额外的 Avalonia 控件：

- **VirtualizingWrapPanel**: 虚拟化环绕面板（开发中）

#### LuYao.Avalonia.Controls.FlagIcon

提供国家旗帜图标控件：

- **FlagIcon**: 显示国家旗帜的图像控件
  - 支持超过 200 个国家/地区代码
  - 自动缩放和优化

### 📖 安装

使用 .NET CLI：

```bash
# 安装行为库
dotnet add package LuYao.Avalonia.Behaviors

# 安装 MiSans 字体
dotnet add package LuYao.Avalonia.Fonts.MiSans.Regular
```

或在你的 `.csproj` 文件中添加：

```xml
<ItemGroup>
  <PackageReference Include="LuYao.Avalonia.Behaviors" Version="0.*" />
  <PackageReference Include="LuYao.Avalonia.Fonts.MiSans.Regular" Version="0.*" />
</ItemGroup>
```

> **注意**: 使用 `Version="0.*"` 会自动获取 0.x 系列的最新版本，避免主版本更新带来的破坏性变更。查看 [NuGet](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) 获取最新版本号。

### 🚀 使用示例

#### 使用 MiSans 字体

在你的 `App.axaml.cs` 中：

```csharp
using Avalonia;
using LuYao.Avalonia.Fonts.MiSans.Regular;

public override void Initialize()
{
    AvaloniaXamlLoader.Load(this);
}

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithMiSansRegularFont()  // 添加这一行
        .LogToTrace();
```

#### 使用 ImageFromFileBehavior

在 XAML 中：

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
        xmlns:behaviors="clr-namespace:LuYao.Avalonia.Behaviors;assembly=LuYao.Avalonia.Behaviors">
    <Image>
        <i:Interaction.Behaviors>
            <behaviors:ImageFromFileBehavior Source="/path/to/image.jpg" 
                                            MaxWidth="800" 
                                            MaxHeight="600" />
        </i:Interaction.Behaviors>
    </Image>
</Window>
```

#### 使用 FlagIcon 控件

在 XAML 中：

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:luyao="https://www.coderbusy.com/luyao">
    <StackPanel Orientation="Horizontal">
        <luyao:FlagIcon Code="CN" Width="32" />
        <luyao:FlagIcon Code="US" Width="32" />
        <luyao:FlagIcon Code="JP" Width="32" />
    </StackPanel>
</Window>
```

### 🛠️ 构建

```bash
# 克隆仓库
git clone https://github.com/coderbusy/luyao-avalonia.git
cd luyao-avalonia

# 恢复依赖
dotnet restore

# 构建项目
dotnet build

# 运行演示（桌面版）
dotnet run --project demo/LuYao.Avalonia.Demo.Desktop
```

### 📄 许可证

本项目采用 [MIT 许可证](LICENSE)。

### 🤝 贡献

欢迎提交问题和拉取请求！

---

## English

LuYao.Avalonia is a collection of useful controls and behaviors for the Avalonia UI framework.

### 📦 NuGet Packages

| Package | Version | Description |
|---------|---------|-------------|
| [LuYao.Avalonia.Behaviors](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) | [![NuGet](https://img.shields.io/nuget/v/LuYao.Avalonia.Behaviors.svg)](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) | Avalonia Behaviors Library |
| [LuYao.Avalonia.Fonts.MiSans.Regular](https://www.nuget.org/packages/LuYao.Avalonia.Fonts.MiSans.Regular) | [![NuGet](https://img.shields.io/nuget/v/LuYao.Avalonia.Fonts.MiSans.Regular.svg)](https://www.nuget.org/packages/LuYao.Avalonia.Fonts.MiSans.Regular) | MiSans Regular Font Integration |

### ✨ Features

#### LuYao.Avalonia.Behaviors

Provides a collection of useful Avalonia behaviors:

- **ImageFromFileBehavior**: Load images from file paths with support for max width and height constraints
  - Automatic image decoding and size optimization
  - Dynamic image source updates

#### LuYao.Avalonia.Fonts.MiSans.Regular

Provides ready-to-use MiSans Regular font integration:

- Configure MiSans font with a single line of code
- No need to manually manage font files

#### LuYao.Avalonia.Controls

Provides additional Avalonia controls:

- **VirtualizingWrapPanel**: Virtualizing wrap panel (in development)

#### LuYao.Avalonia.Controls.FlagIcon

Provides country flag icon control:

- **FlagIcon**: Image control for displaying country flags
  - Supports 200+ country/region codes
  - Automatic scaling and optimization

### 📖 Installation

Using .NET CLI:

```bash
# Install Behaviors library
dotnet add package LuYao.Avalonia.Behaviors

# Install MiSans font
dotnet add package LuYao.Avalonia.Fonts.MiSans.Regular
```

Or add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="LuYao.Avalonia.Behaviors" Version="0.*" />
  <PackageReference Include="LuYao.Avalonia.Fonts.MiSans.Regular" Version="0.*" />
</ItemGroup>
```

> **Note**: Using `Version="0.*"` will automatically get the latest version in the 0.x series, avoiding breaking changes from major version updates. Check [NuGet](https://www.nuget.org/packages/LuYao.Avalonia.Behaviors) for the latest version.

### 🚀 Usage Examples

#### Using MiSans Font

In your `App.axaml.cs`:

```csharp
using Avalonia;
using LuYao.Avalonia.Fonts.MiSans.Regular;

public override void Initialize()
{
    AvaloniaXamlLoader.Load(this);
}

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithMiSansRegularFont()  // Add this line
        .LogToTrace();
```

#### Using ImageFromFileBehavior

In XAML:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
        xmlns:behaviors="clr-namespace:LuYao.Avalonia.Behaviors;assembly=LuYao.Avalonia.Behaviors">
    <Image>
        <i:Interaction.Behaviors>
            <behaviors:ImageFromFileBehavior Source="/path/to/image.jpg" 
                                            MaxWidth="800" 
                                            MaxHeight="600" />
        </i:Interaction.Behaviors>
    </Image>
</Window>
```

#### Using FlagIcon Control

In XAML:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:luyao="https://www.coderbusy.com/luyao">
    <StackPanel Orientation="Horizontal">
        <luyao:FlagIcon Code="CN" Width="32" />
        <luyao:FlagIcon Code="US" Width="32" />
        <luyao:FlagIcon Code="JP" Width="32" />
    </StackPanel>
</Window>
```

### 🛠️ Building

```bash
# Clone the repository
git clone https://github.com/coderbusy/luyao-avalonia.git
cd luyao-avalonia

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the demo (Desktop)
dotnet run --project demo/LuYao.Avalonia.Demo.Desktop
```

### 📄 License

This project is licensed under the [MIT License](LICENSE).

### 🤝 Contributing

Issues and pull requests are welcome!