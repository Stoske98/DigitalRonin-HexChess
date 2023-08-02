﻿
using Newtonsoft.Json;
using System.Collections.Generic;

public interface ISubscribe
{
    void RegisterEvents();
    void UnregisterEvents();
}

public interface IUpgradable
{
    void Upgrade();
}



