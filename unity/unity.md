## Unity

### 前言：

> 一时头脑发热的投入，java游戏服务端转去学习unity开发。不知能坚持多久。
> 
> 这里写一句话给自己：岂不闻天无绝人之路，只要我想走，路就在脚下。

#### PC端如何打出安装包

> unity发布Pc版本  然乎使用inno setup or nsis。

unity组件
- Tilemap（瓦片地图）是一种用于创建2D游戏地图的功能。Tilemap系统允许开发者通过将图块（Tiles）排列在网格中来快速构建复杂的2D地图。Unity的Tilemap功能提供了简单且高效的方式来处理大量2D图块，同时还可以方便地编辑和管理地图。
- collider 碰撞


#### VS-CODE 插件
C#
Chinese
Debugger for unity
Unity
Unity Code Snippets
Unity Tools


#### unity C#脚本生命周期
##### 初始化阶段
- Awake() (针对游戏对象) 执行时机:创建游戏对象的时候立即执行一次 作用:无论脚本是否被启动都会执行。
- OnEnable()和OnDisable() 这两个方法在脚本被启用或禁用时调用，你可以在这里编写处理启用和禁用事件的代码。
- Start() 执行时机:在脚本实例的生命周期中只调用一次，紧跟在Awake()之后。通常用于初始化需要在所有对象都Awake()之后进行的变量或逻辑。
  注意：如果脚本所在的GameObject在场景开始时不是激活的，那么Start()将在GameObject被激活时调用
- FixedUpdate() 按照固定的时间间隔调用，用于物理计算。通常用于移动刚体或其他需要精确时间步长的操作。
调用频率由Time.fixedDeltaTime确定，并且与帧率无关。
- Update() 每帧调用一次,用于处理游戏逻辑 调用频率和帧率相关
- LateUpdate() 在所有update()之后调用,用于处理需要其他对象更新后进行的逻辑，如相机跟随
- OnGUI() 用于绘制用户界面元素，在较新的Unity版本中，通常建议使用UGUI（Unity的图形用户界面系统）来创建用户界面，而不是使用OnGUI()。
- OnDestroy() 当MonoBehaviour实例或包含它的GameObject被销毁时调用。用于执行任何必要的清理工作，例如释放资源或取消事件订阅。