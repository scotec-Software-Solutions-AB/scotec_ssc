#region

using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Scotec.Transactions;
using Scotec.XMLDatabase.DAL;
using Scotec.XMLDatabase.Notification;
using Scotec.XMLDatabase.Rules;

#endregion

namespace Scotec.XMLDatabase.ReaderWriter.Xml;

/// <summary>
///     Summary description for XmlDataDocument.
/// </summary>
public sealed class XmlDataDocument : IDataDocument
                                      //, IDataQuery 
                                      , IDataChangeNotifier
                                      , IDataRuleTrigger
                                      , IDataSession
{
    private readonly IDataRuleTrigger _ruleTrigger = new DataRuleTrigger();
    private readonly Guid _sessionId;
    private bool _dirty;
    private readonly IDataChangeNotifier _notifier;
    private bool _openedOrCreated;
    private bool _previousTrigger;
    private EDataSessionMode _sessionMode = EDataSessionMode.Closed;
    private ITransactionHandler _transactionHandler;
    private readonly List<IDataUpdater> _updaters = new();
    private XmlSchema _xmlSchema;
    private NameTable _xmlSchemaFiles;
    private IDictionary<XmlQualifiedName, XmlSchemaType> _xmlSchemaTypes;
    private readonly IDictionary<string, Stream> _rawSchemas = new Dictionary<string, Stream>();

    #region Contructor

    public XmlDataDocument()
    {
        _sessionId = Guid.Empty;
        _notifier = new DataChangeNotifier(this);
    }

    #endregion Contructor

    #region IDataChangeNotifier Member

    event OnDataChangeEventHandler IDataChangeNotifier.OnDataChange
    {
        add => OnDataChange += value;
        remove => OnDataChange -= value;
    }

    #endregion

    #region IDataDocument Implementation

    public void RegisterUpdaters(IEnumerable<IDataUpdater> updaters)
    {
        _updaters.AddRange(updaters);
    }

    bool IDataDocument.Dirty => InternalDirty;

    bool IDataDocument.UseDefaultTransactionHandler
    {
        get => UseDefaultTransactionHandler;
        set => UseDefaultTransactionHandler = value;
    }

    ITransactionHandler IDataDocument.TransactionHandler
    {
        get => TransactionHandler;
        set => TransactionHandler = value;
    }

    IDataSchema IDataDocument.Schema => null;

    event OnDirtyDataEventHandler IDataDocument.OnDirty
    {
        add => OnDirtyDocument += value;
        remove => OnDirtyDocument -= value;
    }

    event OnBevorDataUpdateEventHandler IDataDocument.BevorUpdate
    {
        add => OnBevorUpdate += value;
        remove => OnBevorUpdate -= value;
    }

    event OnAfterDataUpdateEventHandler IDataDocument.AfterUpdate
    {
        add => OnAfterUpdate += value;
        remove => OnAfterUpdate -= value;
    }

    void IDataDocument.OpenDocument(string source, Uri schema)
    {
        OpenDocument(source, schema);
    }

    void IDataDocument.OpenDocument(Stream source, Uri schema)
    {
        OpenDocument(source, schema);
    }

    void IDataDocument.CreateDocument(string source, string root, Uri schema)
    {
        CreateDocument(source, schema, root);
    }

    void IDataDocument.SaveDocument()
    {
        SaveDocument();
    }

    void IDataDocument.SaveDocument(Stream target)
    {
        SaveDocument(target);
    }

    void IDataDocument.SaveDocumentAs(string target)
    {
        SaveDocumentAs(target);
    }

    void IDataDocument.Close()
    {
        // Call the session's Close() to fire the "SessionClose" event.
        (this as IDataSession).Close();

        Close();
    }

    //IDataObject IDataDocument.Root
    //{
    //    get{return (IDataObject)this.GetRoot();}
    //}

    IDataSession IDataDocument.CreateSession(EDataSessionMode mode)
    {
        return this;
    }

    IDataSession IDataDocument.GetSession(Guid id)
    {
        if (id == _sessionId)
        {
            return this;
        }

        return null;
    }

    public bool IsTriggerEnabled
    {
        get => ((IDataChangeNotifier)this).TriggerEnabled;
        set => ((IDataChangeNotifier)this).TriggerEnabled = value;
    }

    private event OnDirtyDataEventHandler OnDirtyDocument;
    private event OnBevorDataUpdateEventHandler OnBevorUpdate;
    private event OnAfterDataUpdateEventHandler OnAfterUpdate;

    #endregion Interface Implementation

    #region IDataSession Member

    Guid IDataSession.Id => _sessionId;

    EDataSessionMode IDataSession.Mode => _sessionMode;

    EDataWriteMode IDataSession.WriteMode
    {
        get => EDataWriteMode.Default;
        set { }
    }

    IDataDocument IDataSession.Document => this;

    IDataObject IDataSession.Root => GetRoot();

    bool IDataSession.Dirty => InternalDirty;

    IDatabaseObjectHandling IDataSession.ObjectHandling => throw new NotImplementedException("Not implemented");

    IDataFactory IDataSession.DataFactory => throw new NotImplementedException("Not implemented");

    event OnDirtyDataSessionEventHandler IDataSession.OnDirty
    {
        add => OnDirtySession += value;
        remove => OnDirtySession -= value;
    }

    event OnCloseDataSessionEventHandler IDataSession.OnClose
    {
        add => OnClose += value;
        remove => OnClose -= value;
    }

    void IDataSession.Save()
    {
        SaveDocument();
    }

    void IDataSession.Save(Stream target)
    {
        SaveDocument(target);
    }

    void IDataSession.Close()
    {
        if (OnClose != null)
        {
            OnClose(this);
        }
    }

    void IDataSession.DeleteObject(IDataObject dataObject)
    {
        throw new NotImplementedException("Not implemented");
    }

    public IDataObject CopyTo(IDataObject dataObject, IDataList targetDataList, int index)
    {
        var changeNotifier = (IDataChangeNotifier)this;
        var previousTrigger = changeNotifier.TriggerEnabled;

        changeNotifier.TriggerEnabled = false;
        changeNotifier.Lock();

        try
        {
            var xmlDataObject = (XmlDataObject)dataObject;
            var xmlTargetDataList = (XmlDataObject)targetDataList;

            return DataObjectFactory.CloneDataObject(xmlDataObject.Element, xmlTargetDataList.Element, xmlDataObject.SchemaType, index);
        }
        finally
        {
            changeNotifier.Unlock();
            changeNotifier.TriggerEnabled = previousTrigger;
        }
    }

    private event OnDirtyDataSessionEventHandler OnDirtySession;
    private event OnCloseDataSessionEventHandler OnClose;

    #endregion

    #region IDataChangeNotifier Implementation

    bool IDataChangeNotifier.TriggerEnabled
    {
        get => _notifier.TriggerEnabled;
        set => _notifier.TriggerEnabled = value;
    }

    void IDataChangeNotifier.AddChangedObject(IDataObject obj, EDataChangeType type)
    {
        _notifier.AddChangedObject(obj, type);
    }

    void IDataChangeNotifier.Lock()
    {
        _notifier.Lock();
    }

    void IDataChangeNotifier.Unlock()
    {
        _notifier.Unlock();
    }

    public event OnDataChangeEventHandler OnDataChange;

    #endregion IDataChangeNotifier Implementation

    #region IDataRuleTrigger Members

    void IDataRuleTrigger.RegisterRule(IDataRule rule)
    {
        _ruleTrigger.RegisterRule(rule);
    }

    void IDataRuleTrigger.UnregisterRule(IDataRule rule)
    {
        _ruleTrigger.UnregisterRule(rule);
    }

    void IDataRuleTrigger.RunRules(IDictionary<IDataObject, IDataChangedObject> changedObjects)
    {
        if (changedObjects.Count > 0)
        {
            (this as IDataChangeNotifier).Lock();

            try
            {
                _ruleTrigger.RunRules(changedObjects);
            }
            finally
            {
                (this as IDataChangeNotifier).Unlock();
            }
        }
    }

    #endregion

    #region XmlDataDocument Implementation

    private static int s_validationWarnings;
    private static int s_validationErrors;

    internal string XmlFile { get; private set; }

    /// <summary>
    ///     Returns the documents namespace URI.
    /// </summary>
    //		public string NamespaceURI
//		{
//			get{return _xmlDocument.NamespaceURI;}
//		}
    public XmlDocument Document { get; private set; }

    /// <summary>
    ///     Returns the namespace manager.
    /// </summary>
    public XmlNamespaceManager NamespaceManager { get; private set; }

    internal XmlDataObjectFactory DataObjectFactory { get; private set; }

    internal Version SchemaVersion => new(DataObjectFactory.GetSchemaReflectionInfo(_xmlSchema)["Version"]);

    internal Version FileSchemaVersion
    {
        get
        {
            var instruction = ProcessingInstruction;

            if (instruction != null)
            {
                XmlNode tempNode = Document.CreateElement("temp");
                tempNode.InnerXml = "<attribute " + instruction.Data + "/>";
                foreach (XmlAttribute attrib in tempNode.FirstChild.Attributes)
                {
                    if (attrib.Name == "schemaVersion")
                    {
                        return new Version(attrib.Value);
                    }
                }
            }

            // Just a default value. Newer application should never reach this code.
            return new Version();
        }
        set
        {
            var instruction = ProcessingInstruction;

            if (instruction == null)
            {
                // For backward compatibility. Newer application should never reach this code.
                AddProcessingInstructions();
            }
            else
            {
                var data = new StringBuilder();
                XmlNode tempNode = Document.CreateElement("temp");
                tempNode.InnerXml = "<attributes " + instruction.Data + "/>";
                string separator = null;
                foreach (XmlAttribute attrib in tempNode.FirstChild.Attributes)
                {
                    if (attrib.Name == "schemaVersion")
                    {
                        attrib.Value = value.ToString();
                    }

                    data.Append(separator);
                    data.Append(attrib.Name);
                    data.Append("=\"");
                    data.Append(attrib.Value);
                    data.Append("\"");
                    if (separator == null)
                    {
                        separator = ", ";
                    }
                }

                instruction.Data = data.ToString();
            }
        }
    }

    private XmlProcessingInstruction ProcessingInstruction
    {
        get
        {
            foreach (XmlNode node in Document.ChildNodes)
            {
                if (node is XmlProcessingInstruction)
                {
                    var instruction = node as XmlProcessingInstruction;

                    // Get the "XMLDatabase" processing instruction. Test for "INCODIO" since this has been set 
                    // in privious version due to a bug.
                    switch (instruction.Target)
                    {
                        case "XMLDatabase":
                            return instruction;
                        case "SCHEMA": // SCHEMA has been replaced by "XMLDatabase".
                        case "INCODIO":
                            // Has been added accidentely due to a bug in a priivious version. Replace with "SCHEMA".
                            var value = instruction.Value;
                            Document.RemoveChild(instruction);
                            instruction = AddProcessingInstructions();
                            instruction.Value = value;
                            return instruction;
                    }
                }
            }

            return null;
        }
    }

    public bool InternalDirty
    {
        get => _dirty;
        set
        {
            if (_dirty == value)
            {
                return;
            }

            _dirty = value;

            if (OnDirtyDocument != null)
            {
                OnDirtyDocument(this);
            }

            if (OnDirtySession != null)
            {
                OnDirtySession(this);
            }
        }
    }

    internal bool UseDefaultTransactionHandler { get; set; } = true;

    internal ITransactionHandler TransactionHandler
    {
        get => _transactionHandler;
        set
        {
            if (_openedOrCreated)
            {
                throw new Exception(
                    "Document already created or opened.\nSetting the transactionHandler is not allowed!");
            }

            _transactionHandler = value;
        }
    }

    #region Transaction event-handling

    private void OnAfterCommitTransaction()
    {
        //if(_notify != null)
        //	_notify.Unlock();
    }

    private void OnBeforeCommitTransaction()
    {
        //if(_notify != null)
        //	_notify.Lock();
    }

    private void OnAfterRollBackTransaction()
    {
        if (_notifier != null)
        {
            _notifier.Unlock();
        }

        (this as IDataChangeNotifier).TriggerEnabled = _previousTrigger;
    }

    private void OnBeforeRollBackTransaction()
    {
        _previousTrigger = (this as IDataChangeNotifier).TriggerEnabled;
        (this as IDataChangeNotifier).TriggerEnabled = false;

        if (_notifier != null)
        {
            _notifier.Lock();
        }
    }

    #endregion Transaction event-handling

    public ITransactionManager TransactionManager { get; set; }

    /// <summary>
    ///     Creates a new document.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="root"></param>
    /// <param name="schema"></param>
    private void CreateDocument(string source, Uri schema, string root)
    {
        XmlFile = source;

        Initialize();
        _openedOrCreated = true;

        LoadSchemaInfo(schema);

        var elementName = new XmlQualifiedName(root, _xmlSchema.TargetNamespace);
        var schemaElement = (XmlSchemaElement)_xmlSchema.Elements[elementName];

        var type = schemaElement.ElementSchemaType;
        if (string.IsNullOrEmpty(type.Name))
        {
            throw new Exception("No type has been assigned to the root object.");
        }

        NamespaceManager.AddNamespace("dm", _xmlSchema.TargetNamespace);

        var declaration = Document.CreateXmlDeclaration("1.0", "UTF-8", null);
        Document.AppendChild(declaration);

        var element = Document.CreateElement(root, _xmlSchema.TargetNamespace);
        Document.AppendChild(element);
        AddProcessingInstructions();

        var attrib = Document.CreateAttribute("xsi:type", NamespaceManager.LookupNamespace("xsi"));
        attrib.Value = type.Name;
        element.Attributes.SetNamedItem(attrib);

        DataObjectFactory.CreateDataObject(this, Document.DocumentElement);

        _sessionMode = EDataSessionMode.Write;
    }

    /// <summary>
    ///     Opens an existing document from file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="schema"></param>
    private void OpenDocument(string source, Uri schema)
    {
        XmlFile = source;

        using (var stream = new FileStream(source, FileMode.Open, FileAccess.Read))
        {
            OpenDocument(stream, schema);
        }
    }

    /// <summary>
    ///     Opens an existing document from a steam
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="schema"></param>
    private void OpenDocument(Stream stream, Uri schema)
    {
        Document = new XmlDocument();

        Initialize();
        _openedOrCreated = true;

        LoadSchemaInfo(schema);
        NamespaceManager.AddNamespace("dm", _xmlSchema.TargetNamespace);

        var settings = new XmlReaderSettings();
        settings.ValidationEventHandler += ValidationCallBack;
        settings.ValidationType = ValidationType.None;
        settings.ValidationFlags = XmlSchemaValidationFlags.None;

        using (var xmlReader = XmlReader.Create(stream, settings))
        {
            Document.Load(xmlReader);
        }

        var cancelArg = new CancelEventArgs(false);
        var updater = new XmlUpdater();
        if (updater.NeedsUpdate(this))
        {
            if (OnBevorUpdate != null)
            {
                OnBevorUpdate(Document, cancelArg);
            }

            if (!cancelArg.Cancel)
            {
                updater.Update(this, _updaters);
            }
            else
            {
                throw new DataException(EDataError.UpdateCancelled);
            }
        }
        else
        {
            cancelArg.Cancel = true;
        }

        // elements with attribute id for caching
        ((IDataQuery)GetRoot()).Execute("//*[@id]");

        _sessionMode = EDataSessionMode.Write;

        if (!cancelArg.Cancel && OnAfterUpdate != null)
        {
            OnAfterUpdate(this);
        }

        try
        {
            //ValidateDocument();
        }
        catch (Exception)
        {
            //TODO: Add error message.
        }
    }

    /// <summary>
    ///     Saves the current document.
    /// </summary>
    private void SaveDocument()
    {
        if (string.IsNullOrEmpty(XmlFile))
        {
            throw new DataException(EDataError.Document, "Missing file name. Database can be saved to a stream only.");
        }

        using (var stream = new FileStream(XmlFile, FileMode.Create))
        {
            SaveDocument(stream);
        }
    }

    /// <summary>
    ///     Saves the document to a stream
    /// </summary>
    private void SaveDocument(Stream target)
    {
        using (var xmlWriter = XmlWriter.Create(target, new XmlWriterSettings { Indent = true }))
        {
            Document.Save(xmlWriter);
        }

        //Document.Save(target);

        InternalDirty = false;
    }

    /// <summary>
    ///     Saves the current document at the given location.
    /// </summary>
    /// <param name="file"></param>
    private void SaveDocumentAs(string file)
    {
        XmlFile = file;
        SaveDocument();
    }

    /// <summary>
    ///     Closes the current document.
    /// </summary>
    private void CloseDocument()
    {
        SaveDocument();
        Close();
    }

    private void ValidateDocument()
    {
#if DEBUG
        //CommonLogging.AddEntry( "Validating file '" + XmlFile + "'...", MessageType.Info );

        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(_xmlSchema);
        schemaSet.Compile();

        Document.Schemas = schemaSet;

        s_validationWarnings = 0;
        s_validationErrors = 0;

        Document.Validate(OnDocumentValidationEvent);

        //DefaultApplicationLog.AddEntry( "Validation complete -- "
        //                                + validationErrors + " errors, "
        //                                + validationWarnings + " warnings", MessageType.Info );
#endif
    }

    private static void OnDocumentValidationEvent(object sender, ValidationEventArgs e)
    {
        if (e.Severity == XmlSeverityType.Warning)
        {
            s_validationWarnings++;

            //DefaultApplicationLog.AddEntry( "Validation warning in '"
            //                                + sender + "': " + e.Message, MessageType.Warning );
        }
        else
        {
            s_validationErrors++;

            //DefaultApplicationLog.AddEntry( "Validation error in '"
            //                                + sender + "': " + e.Message, MessageType.Error );
        }
    }

    /// <summary>
    ///     Returns the root node of the current document.
    /// </summary>
    /// <returns></returns>
    private IDataObject GetRoot()
    {
        // Get the node
        var xmlElement = Document.DocumentElement;

        // Create the element
        return DataObjectFactory.GetDataObject(xmlElement.Name, null);
    }

    /// <summary>
    ///     Returns the schema type for an element specified in typeName.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public XmlSchemaType GetSchemaType(XmlQualifiedName typeName)
    {
        XmlSchemaType type;
        if (_xmlSchemaTypes.TryGetValue(typeName, out type))
        {
            return type;
        }

        throw new Exception("Unknow type " + typeName + ".");
    }

    //private void LoadSchemaFromFile(Uri uri)
    //{
    //    using var reader = new XmlTextReader(uri.LocalPath);

    //    //_xmlSchema = XmlSchema.Read(reader, ValidationCallBack);
    //    LoadSchemaInfo(uri.LocalPath);
    //}

    /// <summary>
    ///     Initializes the document.
    /// </summary>
    private void Initialize()
    {
        // Create new XML document.
        Document = new XmlDocument();

        // Craete the maps to hold the attribute andobject schema descriptions.
        _xmlSchemaTypes = new Dictionary<XmlQualifiedName, XmlSchemaType>();
        _xmlSchemaFiles = new NameTable();

        // Create namespace manager.
        NamespaceManager = new XmlNamespaceManager(Document.NameTable);

        // Add namespaces to namespace manager.
        NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        //_namespaceManager.AddNamespace("systecs", "http://www.systecs.com/schemas");

        // Create object factory.
        DataObjectFactory = new XmlDataObjectFactory(this, TransactionManager);

        // Enable notification
        _notifier.OnDataChange += delegate(IDataSession document, IDictionary<IDataObject, IDataChangedObject> changedObjects)
        {
            if (OnDataChange != null)
            {
                OnDataChange(document, changedObjects);
            }
        };

        if (_transactionHandler == null && UseDefaultTransactionHandler)
        {
            try
            {
                var trxManager = TransactionManager;
                if (trxManager.HasTransactionHandler(""))
                {
                    _transactionHandler = trxManager.GetTransactionHandler("");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get DefaultTransactionHandler" + ex);
            }
        }

        InitializeTrxHandler(_transactionHandler);
    }

    private void InitializeTrxHandler(ITransactionHandler trxHandler)
    {
        if (trxHandler != null)
        {
            trxHandler.OnAfterCommitTransaction +=
                OnAfterCommitTransaction;
            trxHandler.OnBeforeCommitTransaction +=
                OnBeforeCommitTransaction;
            trxHandler.OnAfterRollBackTransaction +=
                OnAfterRollBackTransaction;
            trxHandler.OnBeforeRollBackTransaction +=
                OnBeforeRollBackTransaction;
        }
    }

    private void DeinitializeTrxHandler(ITransactionHandler trxHandler)
    {
        if (trxHandler != null)
        {
            trxHandler.OnAfterCommitTransaction -=
                OnAfterCommitTransaction;
            trxHandler.OnBeforeCommitTransaction -=
                OnBeforeCommitTransaction;
            trxHandler.OnAfterRollBackTransaction -=
                OnAfterRollBackTransaction;
            trxHandler.OnBeforeRollBackTransaction -=
                OnBeforeRollBackTransaction;
        }
    }

    public void OnXmlNodeChanged(XmlNode node)
    {
        // Set the dirty flag if the XML document has been changed.
        InternalDirty = true;
    }

    private static Stream CopyStream(Stream stream)
    {
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        stream.Position = 0;
        memoryStream.Position = 0;

        return memoryStream;
    }

    private XmlSchema LoadSchema(Assembly assembly, string path, Dictionary<string, XmlSchema> schemas)
    {
        var fragments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (schemas.TryGetValue(fragments.Last().ToLower(), out var loadedSchema))
        {
            return loadedSchema;
        }

        // Do not dispose the stream here. It will be used later.
        var stream = GetSchema(assembly, string.Join(".", fragments));
        // Copy the stream. Otherwise it would be closed by the text reader.
        using var textReader = new XmlTextReader(CopyStream(stream) );

        var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema};
        using var reader = XmlReader.Create(textReader, settings);
        var schema = XmlSchema.Read(reader, ValidationCallBack);

        schemas.Add(fragments.Last().ToLower(), schema);
        _rawSchemas.Add(fragments.Last().ToLower(), stream);

        foreach (var include in schema.Includes.OfType<XmlSchemaInclude>())
        {
            var includedSchema = LoadSchema(assembly, $"{string.Join("/", fragments.Take(fragments.Length - 1))}/{include.SchemaLocation}", schemas);
            if (includedSchema != null)
            {
                include.Schema = includedSchema;
            }
        }

        return schema;

        static Stream GetSchema(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Resource not found", resourceName);
            }

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }

    private XmlSchema LoadSchema(string schemaFile, Dictionary<string, XmlSchema> schemas)
    {
        if (schemas.TryGetValue(schemaFile.ToLower(), out var loadedSchema))
        {
            return loadedSchema;
        }

        using var fileStream = File.OpenRead(schemaFile);
        // Do not dispose the stream here. It will be used later.
        var stream = new MemoryStream();
        fileStream.CopyTo(stream);
        stream.Position = 0;

        // Copy the stream. Otherwise it would be closed by the text reader.
        var textReader = new XmlTextReader(CopyStream(stream) );
        var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema, CloseInput = false};
        var reader = XmlReader.Create(textReader, settings);
        var schema = XmlSchema.Read(reader, ValidationCallBack);

        schemas.Add(schemaFile.ToLower(), schema);
        _rawSchemas.Add(schemaFile.ToLower().ToLower(), stream);

        var schemaPath = Path.GetDirectoryName(schemaFile);
        foreach (var include in schema.Includes.OfType<XmlSchemaInclude>())
        {
            var includedSchema = LoadSchema(Path.Combine(schemaPath, include.SchemaLocation), schemas);
            if (includedSchema != null)
            {
                include.Schema = includedSchema;
            }
        }

        return schema;
    }

    private XmlSchema LoadSchema(Uri schemaFile, Dictionary<string, XmlSchema> schemas)
    {
        var assemblyName = PackUriHelper.GetPackageUri(schemaFile).LocalPath.Trim('/').Split(';').First();
        var partUri = PackUriHelper.GetPartUri(schemaFile).OriginalString.Trim('/');

        var assembly = Assembly.Load(assemblyName);
        return LoadSchema(assembly, $"{assemblyName}/{partUri}", schemas);
    }

    /// <summary>
    ///     Loads all schema info for the given an included schemas.
    ///     The schema will be compiled if not already done.
    /// </summary>
    //private void LoadSchemaInfo( XmlSchema schema )
    private void LoadSchemaInfo(Uri schemaFile)
    {
        var schemas = new Dictionary<string, XmlSchema>();

        if (_xmlSchema == null)
        {
            _xmlSchema = schemaFile.Scheme == "pack" 
                ? LoadSchema(schemaFile, schemas) 
                : LoadSchema(schemaFile.LocalPath, schemas);
        }

        var schemaSet = new XmlSchemaSet();
        foreach (var schema in schemas.Values)
        {
            schemaSet.Add(schema);
        }

        schemaSet.Compile();

        foreach (var schema in schemas)
        {
            // Get the processing instructions.
            // The instructions contain information about the assembly and can be 
            // used to for reflection.
            var stream = _rawSchemas[schema.Key];
            stream.Position = 0;

            using var reader = new XmlTextReader(CopyStream(stream));
            var document = new XmlDocument();
            document.Load(reader);

            IDictionary<string, string> reflectionInfo = new Dictionary<string, string>();
            foreach (XmlNode node in document.ChildNodes)
            {
                if (node is XmlProcessingInstruction && node.Name == "XMLDatabase")
                {
                    // Just put the content into a temporary node.
                    // This makes it easier to extract the attributes.
                    XmlNode tempNode = node.OwnerDocument.CreateElement("temp");
                    tempNode.InnerXml = "<attribute " + node.InnerText + "/>";
                    foreach (XmlAttribute attrib in tempNode.FirstChild.Attributes)
                    {
                        reflectionInfo.Add(attrib.Name, attrib.Value);
                    }
                }
            }

            DataObjectFactory.AddSchemaReflectionInfo(schema.Value, reflectionInfo);

            // Add all namespaces to the namespace manager.
            var serializer = schema.Value.Namespaces;
            var names = serializer.ToArray();
            foreach (var n in names)
            {
                NamespaceManager.AddNamespace(n.Name, n.Namespace);
            }

            _xmlSchemaFiles.Add(schema.Value.SourceUri);

            // Load the type descriptions
            LoadSchemaTypes(schema.Value);
        }
    }

    /// <summary>
    ///     Load all types into the type table.
    /// </summary>
    /// <param name="schema"></param>
    private void LoadSchemaTypes(XmlSchema schema)
    {
        foreach (var so in schema.Items)
        {
            switch (so.GetType().Name)
            {
                case "XmlSchemaSimpleType":
                case "XmlSchemaComplexType":
                {
                    var type = (XmlSchemaType)so;
                    _xmlSchemaTypes[type.QualifiedName] = type;
                }
                    break;
            }
        }
    }

    /// <summary>
    ///     Internal close. Resets all members.
    /// </summary>
    private void Close()
    {
        DeinitializeTrxHandler(_transactionHandler);

        if (DataObjectFactory != null)
        {
            (DataObjectFactory as IDisposable).Dispose();

            DataObjectFactory = null;
        }

        _xmlSchema = null;
        Document = null;
        XmlFile = string.Empty;
        _xmlSchemaTypes = null;
        _xmlSchemaFiles = null;
        NamespaceManager = null;
        DataObjectFactory = null;
        _sessionMode = EDataSessionMode.Closed;
    }

    private XmlProcessingInstruction AddProcessingInstructions()
    {
        var instruction = Document.CreateProcessingInstruction("XMLDatabase", "schemaVersion=\"" + SchemaVersion + "\"");
        Document.InsertBefore(instruction, Document.DocumentElement);

        return instruction;
    }

    internal void CreateBackupFile()
    {
        if (!string.IsNullOrEmpty(XmlFile))
        {
            Document.Save(XmlFile + ".bak");
        }
    }

    /// <summary>
    ///     Validation callback. This method is called from the schema validation or the schema compiler.
    ///     TODO: The validator and the compiler should use different callbacks.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ValidationCallBack(object sender, ValidationEventArgs e)
    {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }

    #endregion XmlDataDocument Implementation
}
