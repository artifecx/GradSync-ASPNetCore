﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources.Messages {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.Messages.ErrorMessages", typeof(ErrorMessages).Assembly);
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
        ///   Looks up a localized string similar to No changes detected..
        /// </summary>
        public static string Error_NoChanges {
            get {
                return ResourceManager.GetString("Error_NoChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User already exists!.
        /// </summary>
        public static string Error_UserExists {
            get {
                return ResourceManager.GetString("Error_UserExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Illegal switch from {0} to {1}..
        /// </summary>
        public static string Error_UserIllegalRoleSwitch {
            get {
                return ResourceManager.GetString("Error_UserIllegalRoleSwitch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect email or password..
        /// </summary>
        public static string Error_UserIncorrectLoginDetails {
            get {
                return ResourceManager.GetString("Error_UserIncorrectLoginDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An has error occurred while logging in..
        /// </summary>
        public static string Error_UserLoginDefault {
            get {
                return ResourceManager.GetString("Error_UserLoginDefault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please check your registered email and spam folder for the verification link. If you haven&apos;t received it, contact us at info.gradsync@gmail.com..
        /// </summary>
        public static string Error_UserNotVerified {
            get {
                return ResourceManager.GetString("Error_UserNotVerified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An has error occurred while registering user..
        /// </summary>
        public static string Error_UserRegistrationDefault {
            get {
                return ResourceManager.GetString("Error_UserRegistrationDefault", resourceCulture);
            }
        }
    }
}
