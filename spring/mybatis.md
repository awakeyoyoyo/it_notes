## Mybatis

### 概述

> mybatis：持久化层的框架【dao层】
>
> springMVC：控制层框架【servlet层】
>
> spring框架：全能



Mybatis简介

- Mybatis是一个‘半自动化’的‘持久化’层‘ORM’框架
  - 持久化层：dao层 与数据库打交道
  - ORM：Object Relational Mapping【对象 关系 映射】
    - 将java对象与数据库的表建立映射关系，操作java对象就可以影响数据库的表数据
  - 半自动化：需要手动编写sql语句
    - mybatis是一个半自动化【可以优化sql的编写】
    - hibernate是全自动化【无需手写sql】
- Mybatis与JDBC对比
  - JDBC中SQL与java代码耦合度高
  - Mybatis将SQL与java代码借耦了



#### Mybatis传递参数问题

##### 单个普通参数

- 可以任意使用普通类型

##### 多个普通参数

- 任意多个参数，都会背mybatis底层封装成一个Map传入，Map的key是param1，param2

##### 命名参数

- 为参数使用@Param起名字，Mybatis就会将这些参数封装进map中

##### POJO参数

- Mybatis支持POJO的成员变量，参数是POJO的属性

##### Map参数

- 传入map的时候，key就是map 的key

####Mybatis参数传递【#与$的区别】

jdbc中

**Statement**：执行sql语句，sql【string】拼接的方式

**PreparedStatement**：执行sql【预编译sql】，参数使用占位符





‘#{key}’：底层执行sql语句对象使用的是PreparedStatemented，防止了sql注入

‘${key}’：底层执行sql语句使用的是Statement，有sql注入的风险

#### 使用场景

‘#’：sql占位符均可以使用

‘$’:   #号解决不了的参数传递问题，均可以交给  from动态化表名

#### Mybatis查询结果问题

> resultType: 设置期望结果集返回类型【类名】
>
> resultType：map

（1）单行数据返回单行对象

（2）多行数据返回对象集合

​	resultType: 设置期望结果集返回类型【类名】

（4）查询单行数据返回Map集合

​	resultType：map 

​	map<key,value> 字段作为map的key，查询结果作为map的value

（5）查询多行数据返回Map集合

​	resultType：map

​	map<integer,pojo>

​	加注解@MapKey("id") 

#### Mybatis自动映射和自定义映射

自动映射【resultType】：自动将表中的字段与类中的属性进行关联映射

自定义映射【resultMap】：自定义返回的查询对象

resultType和resultMap只能使用其中一个。

##### 自动映射

解决不了的问题

​    （1）多表连接查询的时候，需要返回多张表的结果集

​	（2）单表查询，不支持驼峰自动映射【不想为字段定义别名】

##### 自定义映射

 解决多表联合问题

##### association映射

一对一，多对一

![image-20220718171853878](/Users/awakeyoyoyo/Library/Application Support/typora-user-images/image-20220718171853878.png)

可以设置分步查询 select标签

懒加载。  fetchType：lazy    eager 

> 只有对象中的信息被用到才会进行查询



##### collection映射

一对多情况

![image-20220718172917490](/Users/awakeyoyoyo/Library/Application Support/typora-user-images/image-20220718172917490.png)

### mybatis的动态SQL

### mybatis的缓存机制

- 一级缓存（本地缓存 sqlSession级别的缓存）

  本地缓存不能被关闭 但是可以被清空

- 二级缓存（）

- 第三方的缓存

#### 一级缓存

缓存原理

- 第一次获取数据时，先从数据库中加载数据，将数据缓存至Mybatis一级缓存中【缓存底层实现原理Map，key：hashCode+查询的SqlId+编写的sql查询语句+参数】
- 以后再次获取数据时，先从一级缓存中获取，**如未获取到数据**，再从数据库中获取数据。

- **一级缓存五种失效情况**

  1) 不同的SqlSession对应不同的一级缓存

  2) 同一个SqlSession但是查询条件不同

  3) **同一个SqlSession两次查询期间执行了任何一次增删改操作**	

  - 清空一级缓存

  4) 同一个SqlSession两次查询期间手动清空了缓存

  - **sqlSession.clearCache()**

  5) 同一个SqlSession两次查询期间提交了事务

  - sqlSession.commit()

#### Mybatis缓存机制之二级缓存

- 二级缓存【second level cache】概述

  - 二级缓存【全局作用域缓存】
  - SqlSessionFactory级别缓存

- 二级缓存特点

  - 二级缓存默认关闭，需要开启才能使用
  - 二级缓存需要提交sqlSession或关闭sqlSession时，才会缓存。

- 二级缓存使用的步骤:

  ① 全局配置文件中开启二级缓存<setting name="cacheEnabled" value="true"/>

  ② 需要使用二级缓存的**映射文件处**使用cache配置缓存<cache />

  ③ 注意：POJO需要实现Serializable接口