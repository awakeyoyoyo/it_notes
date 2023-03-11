## 面试题

#### 1、谈谈Spring IOC的理解，原理和实现

控制反转：理论思想，原来的对象是由使用者进行控制，有了spring之后，可以把整对象交给spring来管理。依赖注入（DI）：把对应属性的值注入到具体的对象中，可以通过@Autowired，populateBean方法完成属性值的注入

容器：存储具体对象，使用Map结构来存储，在Spring中一般存在三级缓存，一级缓存存储完整的bean对象，二级缓存存储实例化好但未初始化的对象，三级缓存**（循环依赖）**存储创建代理对象的lambda表达式。我们一般获取对象会从一级缓存获取，整个Bean的生命周期从创建到使用到销毁的过程都是由容器来进行管理。**(bean的生命周期)**



##### 聊到ioc容器要涉及容器的创建过程

（1）**初始化容器，设置容器的状态**

（2）**加载解析bean对象**，准备要创建的bean对象的beanDefinition（xml或者注解的解析）可以通过NamePlaceHandler来自定义标签，简化Bean的注入。

（3）**设置BeanFactory的配置信息**，设置容器的awake接口

（4）**执行BeanFactoryPostProcessor方法**，主要用于对读取后的BeanDefinition来进行处理，我们可以定制自己的处理器来插手容器的创建过程，PlaceHolderBeanFactoryPostProcessor处理配置文件中占位符问题

（5）**注册BeanPostProcessor处理器**，此处可以定制自己的处理器来插手Bean的创建过程，常见的就有AOP的实现，还有before after方法的实现。 （游戏里面需要一个独立的事件系统，可以将所有的注入到容器中的监听者和事件收集起来）

（6）**初始化国际化信息**，即语言翻译转换

（7）**初始化事件广播器以及注册监听器**

（8）**注册事件监听者**

（9）完成 BeanFactory 的初始化，同时**实例化单例 且懒加载的Bean**。

##### 聊一下Bean对象的初始化过程

（1）通过反射实例化对象

（2）根据BeanDefinition来填充属性

（3）通过awake接口注入属性

（4）执行BeanPostProcessor的Before方法

（5）执行Bean所配置的init方法

（6）执行BeanPostProcessor的After方法 

（7）得到完整的Bean对象放入容器中

##### Spring事件的处理

> Spring 通过 ApplicationEvent 类和 ApplicationListener 接口提供 ApplicationContext 中的事件处理。如果将实现 ApplicationListener 接口的 bean 部署到上下文中，则每次将 ApplicationEvent 发布到 ApplicationContext 时，都会通知该 bean。本质上，这是标准的观察者设计模式。

#### 2、 谈一下Spring IOC的底层实现

底层实现：其实就是你对Spring IOC的理解，

（1）反射

（2）工厂 BeanFactory

（3）设计模式（模版方法？BeanPostProcessor、BeanFactoryProcessor不影响spring容器的整体流程可定制自身的逻辑）

（4）关键的方法名 refresh()，基本可以理解整个Spring容器的创建过程

#### 3、谈一下Bean的生命周期

##### 聊一下Bean对象的初始化过程

（1）通过反射实例化对象 （可以说反射的实现方式）

（2）根据BeanDefinition来填充属性 （注入依赖，有循环依赖的问题）

（3）通过awake接口注入属性 （注入上下文）

（4）执行BeanPostProcessor的Before方法 （公司的事件组件）

（5）执行Bean所配置的init方法

（6）执行BeanPostProcessor的After方法（spring的AOP实现）

（7）得到完整的Bean对象放入容器中 

#### 4、Spring是如何解决循环依赖的问题

三级缓存

提前暴露对象

实例化和初始化分开

AOP

（1）解释什么是循环依赖，A依赖B，B依赖A，相互成员变量引用

（2）说一下Bean的创建过程：实例化和初始化

​		1、先创建A对象，实例化A对象，然后根据BeanDefinition来填充属性

​		2、到容器中寻找B对象，来注入到A对象中，然后此时找不到B

​		3、实例化B对象，然后根据BeanDefinition来填充属性

​		4、到容器中寻找A对象，来注入到B对象中，然后找不到A就又重新第一步，形成循环。

（3）解决的方式：三级缓存，由上往下的寻找

​       1、一级缓存，存储完整的对象

​	   2、二级缓存，存储实例化好但未初始化的对象

​	   3、三级缓存，存储构建完整代理对象的lambda表达式。（创建代理对象，需要先反射生成普通对象，再进行代理操作。三级缓存就是为了区分两种对象）

#### 5、BeanFactory与FactoryBean有什么区别？

由内部使用的对象实现的接口，这些对象 BeanFactory 本身就是单个对象的工厂。如果 bean 实现此接口，则它将用作对象公开的工厂，而不是直接用作将自身公开的 bean 实例。

注意：实现此接口的 bean 不能用作普通 bean。 FactoryBean以 bean 样式定义，但是为 bean 引用（getObject()）公开的对象始终是它创建的对象。

> BeanFactory是个Factory，也就是IOC容器或对象工厂，FactoryBean是个Bean。在Spring中，所有的Bean都是由BeanFactory(也就是IOC容器)来进行管理的

（2） 创建对象给Spring管理，就需要实现FactoryBean接口

  	      isSingleton 是否单例对象

​			getObjectType 返回对象的类型

   		 getObject：自定义创建对象的过程（new，反射，动态代理）





#### 6、Spring中用到的设计模式

单例模式：Bean默认都是单例的

原型模式：指定作用域为prototype

工厂模式：BeanFactory

模版方法：

策略模式：

观察者模式：事件组件

适配器模式：Adapter

装饰者模式：BeanWrapper

责任链模式：

代理模式：动态代理

委托者模式：

#### 7、Spring的AOP底层实现逻辑

动态代理

aop是ioc的一个扩展功能，先有ioc再有aop，只是在ioc整个流程中新增的一个拓展点

通过BeanPostProcessor的后置处理方法来进行实现。

​	（1）代理对象的创建过程（advice，切面，切点）

​	（2）通过jdk或者cglib的方式来生成代理对象

​	（3）在执行方法调用的时候，会调用到生成的字节码文件中的方法

​	（4）根据之前定义好的通知来生存拦截器链

​	（5）从拦截器链中一次获取每个通知开始执行

#### 8、Spring的事务是如何回滚的

 spring的事务管理是如何实现的？



#### 9、谈一下Spring事务传播





#### 

