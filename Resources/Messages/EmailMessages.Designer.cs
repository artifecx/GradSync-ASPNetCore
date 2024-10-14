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
    public class EmailMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.Messages.EmailMessages", typeof(EmailMessages).Assembly);
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
        ///   Looks up a localized string similar to Hello {0},
        ///
        ///Your password has been successfully reset. Please use the temporary password provided below to log in to your account:
        ///
        ///Temporary Password: {1}
        ///
        ///For your security, we recommend changing this password immediately after logging in.
        ///
        ///If you did not request this password reset, please contact us by replying to this email right away.
        ///
        ///Best regards,
        ///The GradSync Team.
        /// </summary>
        public static string Email_BodyUserPasswordReset {
            get {
                return ResourceManager.GetString("Email_BodyUserPasswordReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {0},
        ///
        ///Thank you for registering with GradSync! To complete your registration and activate your account, please verify your email by clicking the link below:
        ///
        ///Verify Account: {1}
        ///
        ///For security reasons, this link will expire in 1 hour. If the link doesn&apos;t work, you can copy and paste it directly into your browser&apos;s address bar.
        ///
        ///If you did not sign up for a GradSync account, please disregard this email.
        ///
        ///Best regards,
        ///The GradSync Team.
        /// </summary>
        public static string Email_BodyUserRegistration {
            get {
                return ResourceManager.GetString("Email_BodyUserRegistration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {0},
        ///
        ///We received a request to reset the password for your GradSync account. To proceed, please use the link below:
        ///
        ///Reset Password: {1}
        ///
        ///This link will expire in 1 hour for your security. If the link doesn&apos;t work, you can copy and paste it directly into your browser&apos;s address bar.
        ///
        ///If you did not request this reset, please ignore this email—no action is required.
        ///
        ///Thank you,
        ///The GradSync Team.
        /// </summary>
        public static string Email_BodyUserRequestPasswordReset {
            get {
                return ResourceManager.GetString("Email_BodyUserRequestPasswordReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your Password Has Been Reset.
        /// </summary>
        public static string Email_SubjectUserPasswordReset {
            get {
                return ResourceManager.GetString("Email_SubjectUserPasswordReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verify Your Account.
        /// </summary>
        public static string Email_SubjectUserRegistration {
            get {
                return ResourceManager.GetString("Email_SubjectUserRegistration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password Reset Request.
        /// </summary>
        public static string Email_SubjectUserRequestPasswordReset {
            get {
                return ResourceManager.GetString("Email_SubjectUserRequestPasswordReset", resourceCulture);
            }
        }
    }
}