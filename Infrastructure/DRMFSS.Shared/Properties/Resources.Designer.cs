﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DRMFSS.Shared.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DRMFSS.Shared.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Too few parameters: {0}. Must provide a resource assembly name, resource type and resource key in the format &apos;[AssemblyName]|[ResourceType], ResourceKey&apos;..
        /// </summary>
        public static string Expression_TooFewParameters {
            get {
                return ResourceManager.GetString("Expression_TooFewParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Too many parameters: {0}. Must provide a resource assembly name, resource type and resouce key in the format &apos;[AssemblyName]|[ResourceType], ResourceKey&apos;..
        /// </summary>
        public static string Expression_TooManyParameters {
            get {
                return ResourceManager.GetString("Expression_TooManyParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to test LOGOFF.
        /// </summary>
        public static string LogOff {
            get {
                return ResourceManager.GetString("LogOff", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to test LOGON.
        /// </summary>
        public static string LogOn {
            get {
                return ResourceManager.GetString("LogOn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter invalid: {0}. Must provide a resource assembly name and resource type in the format &apos;[AssemblyName]|[ResourceType]&apos;..
        /// </summary>
        public static string Provider_InvalidConstructor {
            get {
                return ResourceManager.GetString("Provider_InvalidConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not support local resources..
        /// </summary>
        public static string Provider_LocalResourcesNotSupported {
            get {
                return ResourceManager.GetString("Provider_LocalResourcesNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find a default resource for {0}..
        /// </summary>
        public static string RM_DefaultResourceNotFound {
            get {
                return ResourceManager.GetString("RM_DefaultResourceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An internal data error has occurred. A duplicate resource entry was found for {0}..
        /// </summary>
        public static string RM_DuplicateResourceFound {
            get {
                return ResourceManager.GetString("RM_DuplicateResourceFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resource not found: {0}.
        /// </summary>
        public static string RM_ResourceNotFound {
            get {
                return ResourceManager.GetString("RM_ResourceNotFound", resourceCulture);
            }
        }
    }
}
