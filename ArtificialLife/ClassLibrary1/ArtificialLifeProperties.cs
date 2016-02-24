using Appccelerate.EventBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System.IO;


namespace ArtificialLife
{
  /// <summary>
  /// use the flags attribute for bitwise operations
  /// </summary>
  [Flags]
  public enum DirectionFlag
  {
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8
  }


  /// <summary>
  /// ArtificialLifeProperties is a singleton class (only one instance can ever be created)
  /// It is used to hold the set of parameters for the application
  /// 
  /// The instance is created the first time any member of the class is referenced. 
  ///  
  /// The class is marked sealed to prevent derivation, which could add instances. 
  /// In addition, the variable is marked readonly, which means that it can be assigned only during static initialization (which is shown here) 
  /// or in a class constructor.
  /// </summary>
  public sealed class ArtificialLifeProperties
  {
    #region Event Broker Setup

    /// <summary>
    /// The instance of the global event broker class used to pass events around the system
    /// </summary>
    public static EventBroker SystemEventBroker = new EventBroker();
   
    #endregion Event Broker Setup


    #region Logging Setup    

    static LogWriter defaultWriter;
    private void SetupLogging()
    {
      if( File.Exists(@"c:\temp\flatfile.log"))
      {
        File.Delete(@"c:\temp\flatfile.log");
      }

      // Formatter
      //TextFormatter briefFormatter = new TextFormatter("Timestamp: {timestamp(local)}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventid}{newline}ActivityId: {property(ActivityId)}{newline}Severity: {severity}{newline}Title:{title}{newline}");          
      TextFormatter briefFormatter = new TextFormatter("{message}");

      // Trace Listener
      //var flatFileTraceListener = new FlatFileTraceListener(
      //  @"C:\Temp\FlatFile.log", 
      //  "----------------------------------------", 
      //  "----------------------------------------", 
      //  briefFormatter);

      var flatFileTraceListener = new FlatFileTraceListener(@"c:\temp\flatfile.log", null, null, briefFormatter);

      // Build Configuration
      var config = new LoggingConfiguration();

      config.AddLogSource("DiskFiles", SourceLevels.All, true).AddTraceListener(flatFileTraceListener);

      defaultWriter = new LogWriter(config);
    }

    public static void Log( string aMessage )
    {
      // Check if logging is enabled before creating log entries.
      if (defaultWriter.IsLoggingEnabled())
      {
        defaultWriter.Write(aMessage);
      }
    }

    #endregion Logging Setup


    #region Singleton Setup Methods

    private static readonly ArtificialLifeProperties instance = new ArtificialLifeProperties();

    public ArtificialLifeProperties() 
    {
      SetupLogging();

      // Check if logging is enabled before creating log entries.
      if (defaultWriter.IsLoggingEnabled())
      {
        defaultWriter.Write("Logging Started");
      }
    }

    public static ArtificialLifeProperties Instance
    {
      get
      {
        return instance;
      }
    }

    #endregion Singleton Setup Methods


    #region Event Name Strings

    /// <summary>
    /// The set of event name URIs used throughout the system 
    /// </summary>

    /// <summary>
    /// Node output event strings
    /// </summary>    
    public const string NodeOutputChangeEvent = "topic://NodeOutput.Change";
    public const string NodeInternalStateChangeEvent = "topic://NodeInternalState.Change";    
    public const string PropagateOutputEvent = "topic://NodeOutput.Propagate";
    public const string PropagationEndEvent = "topic://NodeOutput.PropagationEnd";

    #endregion Event Name Strings
  }

}
