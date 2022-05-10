using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class OrderDisplayViewModel : ViewModelBase, IDisposable, ITabInfoProvider
{
    
    
    public OrderDisplayViewModel(OrderManager manger)
    {
    }

    public string Title => "Bestellungen";
    public bool CanClose => true;

}