#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace DummyUpdater
{
  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  class App : IExternalApplication
  {
    AddInId _appId;

    static DummyUpdater _updater = null;

    public Result OnStartup( UIControlledApplication a )
    {
      // initialise this application's id from the 
      // currently active add-in id taken from the 
      // this application's manifest file:

      _appId = a.ActiveAddInId;

      // instantiate a dummy updater 
      // and register it with Revit

      _updater = new DummyUpdater( _appId );
      _updater.Register();

      Debug.Print( "OnStartup: registered " 
        + _updater.ToString() );

      return Result.Succeeded;
    }

    public Result OnShutdown( UIControlledApplication a )
    {
      if( null != _updater )
      {
        Debug.Print( "OnShutdown: unregistering "
          + _updater.ToString() );

        UpdaterRegistry.UnregisterUpdater( 
          _updater.GetUpdaterId() );

        _updater = null;
      }
      return Result.Succeeded;
    }
  }
}
