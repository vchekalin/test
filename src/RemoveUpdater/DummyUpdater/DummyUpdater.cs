#region Namespaces
using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion

namespace DummyUpdater
{
  /// <summary>
  /// A dummy updater, whose only purpose is 
  /// to cause a warning and be removed.
  /// </summary>
  class DummyUpdater : IUpdater
  {
    /// <summary>
    /// Private GUID indentifying this updater within this add-in.
    /// </summary>
    Guid _guid = new Guid( "641f20e2-9178-49c3-925b-9efa0c4ae2ea" );

    /// <summary>
    /// Updater GUID returned for private GUID plus add-in GUID.
    /// </summary>
    UpdaterId _updaterId = null;

    /// <summary>
    /// Use the lowest possible change priority,
    /// so we get executed last and don't disturb 
    /// anyone else.
    /// </summary>
    ChangePriority _lowestChangePriority
      = ChangePriority.Annotations;

    const string _updaterName
      = "Jeremy's Dummy Updater";

    const string _additionalInformation
      = "Dummy updater, whose only purpose is to cause a warning and be removed.";

    internal DummyUpdater( AddInId addinID )
    {
      _updaterId = new UpdaterId( addinID, _guid );
    }

    /// <summary>
    /// Register this updater with Revit.
    /// </summary>
    internal void Register()
    {
      UpdaterRegistry.RegisterUpdater( this );

      // try to not add any trigger to start with ...
      // we have to add a trigger, otherwise the updater
      // is never executed, and therefore the request 
      // for its presence which triggers the warning
      // is not stored in the document.

      ElementClassFilter wallFilter
        = new ElementClassFilter( typeof( Wall ) );

      UpdaterRegistry.AddTrigger( _updaterId, wallFilter,
        Element.GetChangeTypeAny() );
    }

    public void Execute( UpdaterData data )
    {
      // we need to modify something here, or the 
      // updater requirement will not be saved in 
      // the document.

      Document doc = data.GetDocument();

      //Transaction t = new Transaction( doc,
      //  "Dummy Updater Transaction" );

      //t.Start();
      //t.Commit();

      //if( 0 < data.GetModifiedElementIds().Count )
      //{
      //  foreach( ElementId id in data.GetModifiedElementIds() )
      //  {
      //    Element e = doc.get_Element( id );
      //    Parameter p = e.get_Parameter( BuiltInParameter.ALL_MODEL_MARK ); 
      //    string s = p.AsString();
      //    p.Set( s + " modified by " + ToString() );
      //  }
      //}

      ProjectInfo a
        = new FilteredElementCollector( doc )
          .OfClass( typeof( ProjectInfo ) )
          .FirstElement() as ProjectInfo;


      try
      {
          
          Parameter p = a.get_Parameter(BuiltInParameter.PROJECT_STATUS);
          string s = p.AsString();
          if (s == null) s = string.Empty;
          if (0 < s.Length)
          {
              s += " ";
          }
          p.Set(s + "modified by " + ToString());
         

          Debug.Print("DummyUpdater.Execute: "
                      + GetUpdaterName() + " "
                      + GetUpdaterId());
      }
      catch (Exception ex)
      {
          var td = new TaskDialog("Error");
          td.MainInstruction = ex.Message;
          td.ExpandedContent = ex.ToString();
          td.Show();
      }
    }

    public string GetAdditionalInformation()
    {
      return _additionalInformation;
    }

    public ChangePriority GetChangePriority()
    {
      return _lowestChangePriority;
    }

    public UpdaterId GetUpdaterId()
    {
      return _updaterId;
    }

    public string GetUpdaterName()
    {
      return _updaterName;
    }

    public override string ToString()
    {
      return GetUpdaterName() + " "
        + _updaterId.GetAddInId().GetGUID().ToString() + " "
        + _updaterId.GetGUID().ToString();
    }
  }
}

// C:\Program Files\Autodesk\Revit Architecture 2011\Journals\journal_remove_updater.txt
// C:\tmp\wall_updater_triggered.rvt
// C:\tmp\ > "C:\Program Files\Autodesk\Revit Architecture 2011\Program\Revit.exe" "C:\Program Files\Autodesk\Revit Architecture 2011\Journals\journal_remove_updater.txt"
