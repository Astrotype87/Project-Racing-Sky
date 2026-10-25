using System;
using System.Collections;
using System.Collections.Generic;

public interface IDataHandler
{
    public bool LoadData<T>(ref T data);
    public bool SaveData<T>(T data);
}
