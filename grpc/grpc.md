## Grpc

### 前言

本入门笔记使用的是proto3版本，准备通过几个例子来学习在项目中如何使用grpc。

### 入门Grpc

#### 配置pom文件

##### 依赖

```xml
  <dependency>
      <groupId>io.grpc</groupId>
      <artifactId>grpc-netty-shaded</artifactId>
      <version>1.60.0</version>
      <scope>runtime</scope>
    </dependency>
    <dependency>
      <groupId>io.grpc</groupId>
      <artifactId>grpc-protobuf</artifactId>
      <version>1.60.0</version>
    </dependency>
    <dependency>
      <groupId>io.grpc</groupId>
      <artifactId>grpc-stub</artifactId>
      <version>1.60.0</version>
    </dependency>
    <dependency>
      <groupId>org.apache.tomcat</groupId>
      <artifactId>annotations-api</artifactId>
      <version>6.0.53</version>
      <scope>provided</scope>
    </dependency>
```

##### 插件

```xml

<plugin>
        <groupId>org.xolstice.maven.plugins</groupId>
        <artifactId>protobuf-maven-plugin</artifactId>
        <version>0.6.1</version>
        <configuration>
          <protocArtifact>com.google.protobuf:protoc:3.24.0:exe:${os.detected.classifier}</protocArtifact>
          <pluginId>grpc-java</pluginId>
          <pluginArtifact>io.grpc:protoc-gen-grpc-java:1.60.0:exe:${os.detected.classifier}</pluginArtifact>
        </configuration>
        <executions>
          <execution>
            <goals>
              <goal>compile</goal>
              <goal>compile-custom</goal>
            </goals>
          </execution>
        </executions>
</plugin>
```

#### 定义服务 Defining the service

> grpc允许我们定义四种常见的rpc服务类型。简单请求响应，单通道单请求流式响应（可能有多个响应）、单通道流式请求流式响应（多个请求多个返回）、双通道多流式请求流式响应

##### 1、简单请求响应

> 请求然后等待服务端处理完毕返回数据。就像方法调用一样

```protobuf
  //查找用户信息
  rpc findUserInfo(UserRequest) returns (UserResponse){};
```

##### 2、单通道单请求流式响应

> 服务器端流式RPC，客户端向服务器发送请求并获取流以读取一系列消息。客户端从返回的流中读取，直到不再有消息为止。 
> 
> 在响应类型放置stream关键字

```protobuf
//通过城市查询 每查询到一个就返回一个
 rpc findUserByCity(UserFindRequest) returns (stream UserFindResponse){};
```

##### 3、流式请求流式响应

> 客户端流式RPC，客户端再次使用提供的流写入一系列消息并将其发送到服务器。一旦客户端完成了消息的编写，它就会等待服务器读取所有消息并返回响应。
> 
> 
> 
> 在请求类型之前放置stream关键字来指定客户端流方法。

```protobuf
  //插入多个user 最后返回总的结果
  rpc insertUser(stream UserInsertRequest) returns (UserInsertResponse) {}
```

##### 4、双通道流式请求响应

> 双向流RPC，其中双方使用读写流发送一系列消息。这两个流独立运行，因此客户端和服务器可以按照它们喜欢的任何顺序进行读写：例如，服务器可以等待接收到所有客户端消息后再写响应，也可以交替地读一条消息然后写一条消息，或者其他读写组合。每个流中消息的顺序都会保留下来。
> 
> 在请求和响应之前放置stream关键字来指定这种类型的方法。

```protobuf
  //服务端有新增的user就推送给客户端， filter可以根据实际情况来选择筛选条件
rpc listerUser(stream UserFilterRequest) returns (UserFilterResponse) {}
```

定义好我们的proto文件后，最终文件如下：

```protobuf
syntax = "proto3";

message UserRequest {
  string name = 1;
}

message UserResponse {
  string name = 1;
  string birthday = 2;
  string city = 3;
}

message UserFindRequest {
  string city = 1;
}

message UserFindResponse {
  string name = 1;
  string birthday = 2;
  string city = 3;
}

message UserInsertRequest {
  string name = 1;
  string birthday = 2;
  string city = 3;}

message UserInsertResponse {
  int32 userNum = 1;
}

message UserFilterRequest {
  string name = 1;
  string birthday = 2;
  string city = 3;
}

message UserFilterResponse {
  int32 userNum = 1;
}

service UserService {
  //查找用户信息
  rpc findUserInfo(UserRequest) returns (UserResponse){};
  //通过城市查询 每查询到一个就返回一个
  rpc findUserByCity(UserFindRequest) returns (stream UserFindResponse){};
  //插入多个user 最后返回总的结果
  rpc insertUser(stream UserInsertRequest) returns (UserInsertResponse) {}
  //服务端有新增的user就推送给客户端， filter可以根据实际情况来选择筛选条件
  rpc listerUser(stream UserFilterRequest) returns (UserFilterResponse) {}
}
```

#### 生成java类

idea中的maven下点击protobuf:compile    protobuf:compile-custom

即刻在target目录下找到我们生成出来的java类

服务注册类：UserServiceGrpc

实体类：User.java

将其复制到项目目录下即可开始我们的练习。

#### Client代码



#### Server代码


