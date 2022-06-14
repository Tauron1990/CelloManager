using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using SystemsRx.MicroRx.Subjects;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens;

public static class UiExtensions
{
    public static Button OnClick(this Button btn, View container, Func<IObservable<Unit>, IDisposable> handler)
    {
        var btnEvents = btn.Events();
        return btn.OnConnect(container, () => handler(btnEvents.Clicked));
    }
    
    public static TView OnConnect<TView>(this TView view, View container, Func<IDisposable> handler)
        where TView : View
    { var conEvents = container.Events();
        
        var disposable = new CompositeDisposable();
        disposable.Add(handler());
        disposable.Add(conEvents.Removed.Where(v => v == view).Subscribe(_ => disposable.Dispose()));
        
        return view;
    }
    
    public static MenuBar CreateMenuBar(this View container, Func<MenuItemFactory, IEnumerable<MenuBarItemBuilder>> factory)
    {
        var menuBar = new MenuBar();
        menuBar.Menus = factory(new MenuItemFactory(container)).Select(b => b.Build(menuBar)).ToArray();

        return menuBar;
    }
    
    public sealed class MenuItemFactory
    {
        private readonly View _container;

        public MenuItemFactory(View container)
        {
            _container = container;
        }
        
        public MenuBarItemBuilder NewMenuBarItem() => new(this, _container);

        public MenuItemBuilder NewMenuItem() => new(this, _container);
    }

    public abstract class MenuItemBuilderBase<TBuilder, TItemType>
        where TBuilder : MenuItemBuilderBase<TBuilder, TItemType>
        where TItemType : MenuItem
    {
        private readonly Func<Action, string, TItemType> _itemBuilder;
        private View Container { get; }

        protected MenuItemBuilderBase(MenuItemFactory itemFactory, View container, Func<Action, string, TItemType> itemBuilder)
        {
            _itemBuilder = itemBuilder;
            Container = container;
        }
        
        private string? Label { get; set; }
        
        private Func<IObservable<Unit>, IDisposable>? _onClick { get; set; }

        public TBuilder WithLabel(string label)
        {
            Label = label;
            return (TBuilder)this;
        }

        public TBuilder WithClick(Func<IObservable<Unit>, IDisposable> clickBuilder)
        {
            _onClick = clickBuilder;
            return (TBuilder)this;
        }

        public virtual TItemType Build(MenuBar bar)
        {
            Action click;

            if (_onClick is not null)
            {
                var subject = new Subject<Unit>();
                void Click() => subject.OnNext(Unit.Default);

                click = Click;
                bar.OnConnect(Container, () => new CompositeDisposable(subject, _onClick(subject.AsObservable())));
            }
            else
            {
                click = () => { };
            }

            return _itemBuilder(click, Label ?? string.Empty);
        }
    }
    
    public sealed class MenuItemBuilder : MenuItemBuilderBase<MenuItemBuilder, MenuItem>
    {
        public MenuItemBuilder(MenuItemFactory itemFactory, View container) 
            : base(itemFactory, container, (action, label) => new MenuItem(label, string.Empty, action))
        { }
    }
    
    public sealed class MenuBarItemBuilder : MenuItemBuilderBase<MenuBarItemBuilder, MenuBarItem>
    {
        public MenuBarItemBuilder(MenuItemFactory itemFactory, View container) 
            : base(itemFactory, container, (action, label) => new MenuBarItem(label, string.Empty, action))
        {
        }

        private Func<IEnumerable<MenuItemBuilder>>? ChildMenu { get; set; }

        public MenuBarItemBuilder WithChiledMenu(Func<IEnumerable<MenuItemBuilder>> builder)
        {
            ChildMenu = builder;
            return this;
        }
        
        public override MenuBarItem Build(MenuBar bar)
        {
            var item = base.Build(bar);

            if (ChildMenu is not null)
            {
                item.Children = ChildMenu().Select(mb => mb.Build(bar)).ToArray();
            }
            
            return item;
        }
    }
}