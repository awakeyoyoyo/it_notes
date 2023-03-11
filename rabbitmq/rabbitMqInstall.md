## Mac RabbitMq 安装

#### 1、 更新最新版本的Brew

```
brew update
```

#### 2、执行安装

``` 
brew install rabbitmq
```

- 安装文件路径是在 /usr/local/Cellar/rabbitmq/<版本号>/ , 其实可以在/usr/local/opt/rabbitmq/sbin 进行访问，它也创建一个链接文件在(/usr/local/sbin)

#### 3、创建配置文件 rabbitmq.conf

> brew安装默认不会创建该配置文件

![image-20220802182001665](../images/rabbitmq02.jpeg)

根据官方给的默认路径 创建到对应的地方即可。

[官方配置文件](https://github.com/rabbitmq/rabbitmq-server/blob/v3.8.x/deps/rabbit/docs/rabbitmq.conf.example)



#### 4、启动rabbitMq

```
## 启动rabbitmq
brew services start rabbitmq

## 关闭rabbitmq
brew services stop rabbitmq
```

> 访问http://localhost:15672/即可



#### 5、查看节点配置文件

![image-20220802182001665](../images/rabbitmq03.png)





完毕