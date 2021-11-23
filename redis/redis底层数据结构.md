## Redis底层数据结构

### 简单动态字符串

```C
struct sdshdr{
    //
    int len;
    int free;
    char buf[];
}
```

