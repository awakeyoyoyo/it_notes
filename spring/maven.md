## maven

### 为什么要使用maven？

- 获取jar包
  - 使用maven可以通过配置方式自动下载jar包
- 添加jar包
  - 使用Maven之前，将jar复制到项目工程中的lib包下，每个项目都需要复制到lib包，浪费空间
  - 使用maven后，jar包统一存储在maven的本地仓库，使用坐标方式将jar包引入到项目中
- 可以便于解决jar之间的依赖冲突问题

### 什么是maven？

自动化构建工具：用于java平台的项目构建和依赖管理

依赖管理：jar之间依赖关系，jar包管理问题

> 就是某个jar包依赖其他jar包，所有全部jar包都要用到

##### 项目构建

![image-20220718141454035](/Users/awakeyoyoyo/Library/Application Support/typora-user-images/image-20220718141454035.png)

![image-20220718142111949](/Users/awakeyoyoyo/Library/Application Support/typora-user-images/image-20220718142111949.png)

### maven的核心概念

#### maven的Pom.xml常用标签

- <parent> 设置父工程的坐标 （artifactId（公司或者组织域名倒叙）、groupId（当前项目名称）、version（当前项目版本【快照】））
- <artifactId> 模块的id
- <dependencies> 依赖 

#### maven约定的目录结构

- 项目名
  - src
    - main
      - java
      - resource
    - test
  - pom.xml
  - target[编译运行后生成 class文件]

#### maven的仓库（！！！）

仓库分类

- 本地仓库：为当前计算机提供maven服务
- 远程仓库：为其他计算机提供maven服务
  - 私服：架设在当前局域网环境下，为当前局域网范围内的所有maven工程服务
  - 中央仓库：架设在internet上，为世界上所有maven工程服务
  - 中央仓库的镜像：架设在各大洲 分担压力罢了

仓库中文件类型

- maven的插件
- 第三方框架或者工具的jar
- 自己研发的项目包或者模块

#### maven的坐标（！！！）

坐标由三部分组成 g-a-v

- groupId 项目的名称
- aftifactId 公司或者组织域名倒叙
- Version 版本

#### maven的依赖管理

- 依赖的范围

  ![image-20220718144617338](/Users/awakeyoyoyo/Library/Application Support/typora-user-images/image-20220718144617338.png)

  范围标签：<scope>标签

  - compile【默认值】：在main、test、tomcat【服务器】下都有效
  - test：只在test目录下有效
  - provided：在main、test下均有效，在tomcat【服务器】无效
  - runtime：在main、test都生效

- 依赖的传递性

  - 路径最短者优先
  - 先声明的依赖先声明的

  maven可以自动解决jar包之间的依赖

### maven中的统一管理

语法

```xml
<properties>
  <xxx-version>1.1.1</xxx-version>
</properties>

<version>
	${xxx-version}
</version>
```

#### maven的继承

 #### 为什么要继承

- 如子工程中大部分jar都共同使用，可以提取到父工程中，使用继承的原理在子工程是使用

- 父工程打包方式，必须是pom方式 

  ```xml
  <packaing>pom</packaing>
  pom:父工程必须是pom
  jar:普通工程
  war:web工程
  ```

#### 继承方式

（1）在父工程的pom.xml中倒入jar包，在子工程中统一使用。【所有子工程强制引入父工程的jar包】

（2）将Parent项目中的dependencies标签，用DependecyManagement标签括起来

```xml
<dependecyManagement>
  <dependencies>
  	....
  </dependencies>
</dependecyManagement>
```

在子项目中引入父工程的pom.xml文件路径

```xml
<relativePath>
.....
</relativePath>
```

需要的时候在子项目中，使用依赖，不需要再写version和scope

#### maven的聚合

为什么要使用Maven的聚合？

 只要将子工程聚合到父工程中，就可以实现效果：安装和清除父工程时，子工程也会同步进行相关操作。好管理

maven会按照依赖顺序来自动安装子工程