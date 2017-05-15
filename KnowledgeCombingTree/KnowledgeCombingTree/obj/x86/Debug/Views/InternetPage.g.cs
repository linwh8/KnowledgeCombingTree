﻿#pragma checksum "D:\KnowledgeCombingTree\KnowledgeCombingTree\KnowledgeCombingTree\KnowledgeCombingTree\Views\InternetPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AD2CC602E1C8F5BF830CCA85F50E749E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KnowledgeCombingTree.Views
{
    partial class InternetPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        internal class XamlBindingSetters
        {
            public static void Set_Template10_Controls_PageHeader_Frame(global::Template10.Controls.PageHeader obj, global::Windows.UI.Xaml.Controls.Frame value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::Windows.UI.Xaml.Controls.Frame) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::Windows.UI.Xaml.Controls.Frame), targetNullValue);
                }
                obj.Frame = value;
            }
            public static void Set_Windows_UI_Xaml_Controls_TextBox_Text(global::Windows.UI.Xaml.Controls.TextBox obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
            public static void Set_Windows_UI_Xaml_Controls_ItemsControl_ItemsSource(global::Windows.UI.Xaml.Controls.ItemsControl obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.ItemsSource = value;
            }
            public static void Set_Windows_UI_Xaml_Controls_TextBlock_Text(global::Windows.UI.Xaml.Controls.TextBlock obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
        };

        private class InternetPage_obj17_Bindings :
            global::Windows.UI.Xaml.IDataTemplateExtension,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IInternetPage_Bindings
        {
            private global::KnowledgeCombingTree.Models.TreeNode dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);
            private bool removedDataContextHandler = false;

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.TextBlock obj18;
            private global::Windows.UI.Xaml.Controls.TextBlock obj19;

            public InternetPage_obj17_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 18:
                        this.obj18 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    case 19:
                        this.obj19 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    default:
                        break;
                }
            }

            public void DataContextChangedHandler(global::Windows.UI.Xaml.FrameworkElement sender, global::Windows.UI.Xaml.DataContextChangedEventArgs args)
            {
                 global::KnowledgeCombingTree.Models.TreeNode data = args.NewValue as global::KnowledgeCombingTree.Models.TreeNode;
                 if (args.NewValue != null && data == null)
                 {
                    throw new global::System.ArgumentException("Incorrect type passed into template. Based on the x:DataType global::KnowledgeCombingTree.Models.TreeNode was expected.");
                 }
                 this.SetDataRoot(data);
                 this.Update();
            }

            // IDataTemplateExtension

            public bool ProcessBinding(uint phase)
            {
                throw new global::System.NotImplementedException();
            }

            public int ProcessBindings(global::Windows.UI.Xaml.Controls.ContainerContentChangingEventArgs args)
            {
                int nextPhase = -1;
                switch(args.Phase)
                {
                    case 0:
                        nextPhase = -1;
                        this.SetDataRoot(args.Item as global::KnowledgeCombingTree.Models.TreeNode);
                        if (!removedDataContextHandler)
                        {
                            removedDataContextHandler = true;
                            ((global::Windows.UI.Xaml.Controls.Grid)args.ItemContainer.ContentTemplateRoot).DataContextChanged -= this.DataContextChangedHandler;
                        }
                        this.initialized = true;
                        break;
                }
                this.Update_((global::KnowledgeCombingTree.Models.TreeNode) args.Item, 1 << (int)args.Phase);
                return nextPhase;
            }

            public void ResetTemplate()
            {
            }

            // IInternetPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // InternetPage_obj17_Bindings

            public void SetDataRoot(global::KnowledgeCombingTree.Models.TreeNode newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::KnowledgeCombingTree.Models.TreeNode obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_name(obj.name, phase);
                        this.Update_path(obj.path, phase);
                    }
                }
            }
            private void Update_name(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj18, obj, null);
                }
            }
            private void Update_path(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj19, obj, null);
                }
            }
        }

        private class InternetPage_obj21_Bindings :
            global::Windows.UI.Xaml.IDataTemplateExtension,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IInternetPage_Bindings
        {
            private global::KnowledgeCombingTree.Models.TreeNode dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);
            private bool removedDataContextHandler = false;

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.TextBlock obj22;
            private global::Windows.UI.Xaml.Controls.TextBlock obj23;

            public InternetPage_obj21_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 22:
                        this.obj22 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    case 23:
                        this.obj23 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    default:
                        break;
                }
            }

            public void DataContextChangedHandler(global::Windows.UI.Xaml.FrameworkElement sender, global::Windows.UI.Xaml.DataContextChangedEventArgs args)
            {
                 global::KnowledgeCombingTree.Models.TreeNode data = args.NewValue as global::KnowledgeCombingTree.Models.TreeNode;
                 if (args.NewValue != null && data == null)
                 {
                    throw new global::System.ArgumentException("Incorrect type passed into template. Based on the x:DataType global::KnowledgeCombingTree.Models.TreeNode was expected.");
                 }
                 this.SetDataRoot(data);
                 this.Update();
            }

            // IDataTemplateExtension

            public bool ProcessBinding(uint phase)
            {
                throw new global::System.NotImplementedException();
            }

            public int ProcessBindings(global::Windows.UI.Xaml.Controls.ContainerContentChangingEventArgs args)
            {
                int nextPhase = -1;
                switch(args.Phase)
                {
                    case 0:
                        nextPhase = -1;
                        this.SetDataRoot(args.Item as global::KnowledgeCombingTree.Models.TreeNode);
                        if (!removedDataContextHandler)
                        {
                            removedDataContextHandler = true;
                            ((global::Windows.UI.Xaml.Controls.Grid)args.ItemContainer.ContentTemplateRoot).DataContextChanged -= this.DataContextChangedHandler;
                        }
                        this.initialized = true;
                        break;
                }
                this.Update_((global::KnowledgeCombingTree.Models.TreeNode) args.Item, 1 << (int)args.Phase);
                return nextPhase;
            }

            public void ResetTemplate()
            {
            }

            // IInternetPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // InternetPage_obj21_Bindings

            public void SetDataRoot(global::KnowledgeCombingTree.Models.TreeNode newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::KnowledgeCombingTree.Models.TreeNode obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_name(obj.name, phase);
                        this.Update_description(obj.description, phase);
                    }
                }
            }
            private void Update_name(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj22, obj, null);
                }
            }
            private void Update_description(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj23, obj, null);
                }
            }
        }

        private class InternetPage_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IInternetPage_Bindings
        {
            private global::KnowledgeCombingTree.Views.InternetPage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Template10.Controls.PageHeader obj6;
            private global::Windows.UI.Xaml.Controls.TextBox obj11;
            private global::Windows.UI.Xaml.Controls.TextBox obj12;
            private global::Windows.UI.Xaml.Controls.TextBox obj13;
            private global::Windows.UI.Xaml.Controls.ListView obj16;
            private global::Windows.UI.Xaml.Controls.ListView obj20;

            private InternetPage_obj1_BindingsTracking bindingsTracking;

            public InternetPage_obj1_Bindings()
            {
                this.bindingsTracking = new InternetPage_obj1_BindingsTracking(this);
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 6:
                        this.obj6 = (global::Template10.Controls.PageHeader)target;
                        break;
                    case 11:
                        this.obj11 = (global::Windows.UI.Xaml.Controls.TextBox)target;
                        (this.obj11).LostFocus += (global::System.Object sender, global::Windows.UI.Xaml.RoutedEventArgs e) =>
                            {
                                if (this.initialized)
                                {
                                    // Update Two Way binding
                                    this.dataRoot.ViewModel.SelectedItem.path = (this.obj11).Text;
                                }
                            };
                        break;
                    case 12:
                        this.obj12 = (global::Windows.UI.Xaml.Controls.TextBox)target;
                        (this.obj12).LostFocus += (global::System.Object sender, global::Windows.UI.Xaml.RoutedEventArgs e) =>
                            {
                                if (this.initialized)
                                {
                                    // Update Two Way binding
                                    this.dataRoot.ViewModel.SelectedItem.name = (this.obj12).Text;
                                }
                            };
                        break;
                    case 13:
                        this.obj13 = (global::Windows.UI.Xaml.Controls.TextBox)target;
                        (this.obj13).LostFocus += (global::System.Object sender, global::Windows.UI.Xaml.RoutedEventArgs e) =>
                            {
                                if (this.initialized)
                                {
                                    // Update Two Way binding
                                    this.dataRoot.ViewModel.SelectedItem.description = (this.obj13).Text;
                                }
                            };
                        break;
                    case 16:
                        this.obj16 = (global::Windows.UI.Xaml.Controls.ListView)target;
                        break;
                    case 20:
                        this.obj20 = (global::Windows.UI.Xaml.Controls.ListView)target;
                        break;
                    default:
                        break;
                }
            }

            // IInternetPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
                this.bindingsTracking.ReleaseAllListeners();
                this.initialized = false;
            }

            // InternetPage_obj1_Bindings

            public void SetDataRoot(global::KnowledgeCombingTree.Views.InternetPage newDataRoot)
            {
                this.bindingsTracking.ReleaseAllListeners();
                this.dataRoot = newDataRoot;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::KnowledgeCombingTree.Views.InternetPage obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_Frame(obj.Frame, phase);
                    }
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel(obj.ViewModel, phase);
                    }
                }
            }
            private void Update_Frame(global::Windows.UI.Xaml.Controls.Frame obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Template10_Controls_PageHeader_Frame(this.obj6, obj, null);
                }
            }
            private void Update_ViewModel(global::KnowledgeCombingTree.ViewModels.InternetPageViewModel obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_SelectedItem(obj.SelectedItem, phase);
                    }
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_ChildrenItems(obj.ChildrenItems, phase);
                        this.Update_ViewModel_RootItems(obj.RootItems, phase);
                    }
                }
            }
            private void Update_ViewModel_SelectedItem(global::KnowledgeCombingTree.Models.TreeNode obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_SelectedItem_path(obj.path, phase);
                        this.Update_ViewModel_SelectedItem_name(obj.name, phase);
                        this.Update_ViewModel_SelectedItem_description(obj.description, phase);
                    }
                }
            }
            private void Update_ViewModel_SelectedItem_path(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBox_Text(this.obj11, obj, null);
                }
            }
            private void Update_ViewModel_SelectedItem_name(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBox_Text(this.obj12, obj, null);
                }
            }
            private void Update_ViewModel_SelectedItem_description(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBox_Text(this.obj13, obj, null);
                }
            }
            private void Update_ViewModel_ChildrenItems(global::System.Collections.ObjectModel.ObservableCollection<global::KnowledgeCombingTree.Models.TreeNode> obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_ItemsControl_ItemsSource(this.obj16, obj, null);
                }
            }
            private void Update_ViewModel_RootItems(global::System.Collections.ObjectModel.ObservableCollection<global::KnowledgeCombingTree.Models.TreeNode> obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_ItemsControl_ItemsSource(this.obj20, obj, null);
                }
            }

            private class InternetPage_obj1_BindingsTracking
            {
                global::System.WeakReference<InternetPage_obj1_Bindings> WeakRefToBindingObj; 

                public InternetPage_obj1_BindingsTracking(InternetPage_obj1_Bindings obj)
                {
                    WeakRefToBindingObj = new global::System.WeakReference<InternetPage_obj1_Bindings>(obj);
                }

                public void ReleaseAllListeners()
                {
                }

            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.ThisPage = (global::Windows.UI.Xaml.Controls.Page)(target);
                }
                break;
            case 2:
                {
                    this.AdaptiveVisualStateGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 3:
                {
                    this.VisualStateNarrow = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.VisualStateNormal = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.VisualStateWide = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.pageHeader = (global::Template10.Controls.PageHeader)(target);
                }
                break;
            case 7:
                {
                    this.ChildListGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 8:
                {
                    this.InfoGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 9:
                {
                    this.InfoImage = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 10:
                {
                    this.toggleSwitch1 = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 171 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.toggleSwitch1).Toggled += this.toggleSwitch1_Toggled;
                    #line default
                }
                break;
            case 11:
                {
                    this.path = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 12:
                {
                    this.name = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 13:
                {
                    this.description = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 14:
                {
                    this.Update = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 187 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Update).Click += this.Update_Click;
                    #line default
                }
                break;
            case 15:
                {
                    this.Cancel = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 188 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Cancel).Click += this.Cancel_Click;
                    #line default
                }
                break;
            case 16:
                {
                    this.ChildList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    #line 128 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.ChildList).DragItemsStarting += this.RootList_DragItemsStarting;
                    #line 129 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.ChildList).ItemClick += this.ChildList_ItemClick;
                    #line 130 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.ChildList).DoubleTapped += this.ChildList_DoubleTapped;
                    #line default
                }
                break;
            case 20:
                {
                    this.RootList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    #line 89 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.RootList).DragItemsStarting += this.RootList_DragItemsStarting;
                    #line 92 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.RootList).ItemClick += this.RootList_ItemClick;
                    #line 93 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.RootList).RightTapped += this.RootList_RightTapped;
                    #line default
                }
                break;
            case 24:
                {
                    this.AddNode = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 56 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.AddNode).Click += this.AddNode_Click;
                    #line default
                }
                break;
            case 25:
                {
                    this.Delete = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 58 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.Delete).Drop += this.Delete_Drop;
                    #line 59 "..\..\..\Views\InternetPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.Delete).DragOver += this.Delete_DragOver;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    InternetPage_obj1_Bindings bindings = new InternetPage_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                }
                break;
            case 17:
                {
                    global::Windows.UI.Xaml.Controls.Grid element17 = (global::Windows.UI.Xaml.Controls.Grid)target;
                    InternetPage_obj17_Bindings bindings = new InternetPage_obj17_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot((global::KnowledgeCombingTree.Models.TreeNode) element17.DataContext);
                    element17.DataContextChanged += bindings.DataContextChangedHandler;
                    global::Windows.UI.Xaml.DataTemplate.SetExtensionInstance(element17, bindings);
                }
                break;
            case 21:
                {
                    global::Windows.UI.Xaml.Controls.Grid element21 = (global::Windows.UI.Xaml.Controls.Grid)target;
                    InternetPage_obj21_Bindings bindings = new InternetPage_obj21_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot((global::KnowledgeCombingTree.Models.TreeNode) element21.DataContext);
                    element21.DataContextChanged += bindings.DataContextChangedHandler;
                    global::Windows.UI.Xaml.DataTemplate.SetExtensionInstance(element21, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

