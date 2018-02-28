using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MimeKit;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Core.Fakes;
using Tauron.Application.CelloManager.Logic.RefillPrinter;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.Logic.Core
{
    [Export(typeof(ISettingsModel)), NotShared]
    public sealed class SettingsModel : ModelBase, ISettingsModel
    {

        [Inject]
        public IManagerEnviroment ManagerEnviroment { get; set; }

        public ISettings Settings { get; set; }

        public SettingsModel()
        {
            #if DEBUG
            ManagerEnviroment = new ManagerEnvirtomentFake();
            #endif
        }
        
        public void Save()
        {
            Settings.DefaultPrinter = DefaultPrinter;
            Settings.Dns = Dns;
            Settings.MaximumSpoolHistorie = MaximumSpoolHistorie;
            Settings.PrinterType = PrinterType;
            Settings.Purge = Purge;
            Settings.TargetEmail = TargetEmail;
            Settings.Threshold = Threshold;

            ManagerEnviroment.Save();
        }
        
        public bool CanSave() => HasNoErrors;
        
        public void Cancel() => ManagerEnviroment.Reload();

        public string ErrorText => GetIssuesDictionary().AllValues.FirstOrDefault()?.Message ?? string.Empty;

        public static readonly ObservableProperty DnsProperty = RegisterProperty("Dns", typeof(SettingsModel), typeof(string), new ObservablePropertyMetadata()
                                                                                     .SetValidationRules(new ModelRule(ValidateDns)
                                                                                                         {
                                                                                                             Message = GetMessage(nameof(UIResources.DnsValidationMessage))
                                                                                                         }));
        public string Dns
        {
            get => GetValue<string>(DnsProperty);
            set => SetValue(DnsProperty, value);
        }

        public static readonly ObservableProperty TargetEmailProperty = RegisterProperty("TargetEmail", typeof(SettingsModel), typeof(string), new ObservablePropertyMetadata()
                                                                                     .SetValidationRules(new ModelRule(ValidateEmail)
                                                                                                         {
                                                                                                             Message = GetMessage(nameof(UIResources.TargetEmailValidation))
                                                                                                         }));

        public string TargetEmail
        {
            get => GetValue<string>(TargetEmailProperty);
            set => SetValue(TargetEmailProperty, value);
        }
        public RefillPrinterType PrinterType { get; set; }
        public Dictionary<RefillPrinterType, string> PrinterTypeCaptions => GetSingleton(() => new Dictionary<RefillPrinterType, string> // Fix. Each time new dict.?
                                                                                               {
                                                                                                   {RefillPrinterType.Print, UIResources.RefillPrinterTypePrint},
                                                                                                   {RefillPrinterType.Email, UIResources.RefillPrinterTypeEmail},
                                                                                                   //{ExampleEnum.None, "Hidden in UI"},
                                                                                               });


        private bool _purge;
        public bool Purge
        {
            get => _purge;
            set => SetProperty(ref _purge, value);
        }
        public static readonly ObservableProperty ThresholdProperty = RegisterProperty("Threshold", typeof(SettingsModel), typeof(int), new ObservablePropertyMetadata()
                                                                                             .SetValidationRules(new ModelRule(ValidateNumber)
                                                                                                                 {
                                                                                                                     Message = GetMessage(nameof(UIResources.ThresholdValidation))
                                                                                                                 }));

        public int Threshold
        {
            get => GetValue<int>(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }
        public string DefaultPrinter { get; set; }
        public static readonly ObservableProperty MaximumSpoolHistorieProperty = RegisterProperty("MaximumSpoolHistorie", typeof(SettingsModel), typeof(int), new ObservablePropertyMetadata()
                                                                                           .SetValidationRules(new ModelRule(ValidateNumber)
                                                                                                               {
                                                                                                                   Message = GetMessage(nameof(UIResources.MaximumSpoolHistorieValidation))
                                                                                                               }));
        public int MaximumSpoolHistorie
        {
            get => GetValue<int>(MaximumSpoolHistorieProperty);
            set => SetValue(MaximumSpoolHistorieProperty, value);
        }

        public override void BuildCompled()
        {
            Settings = ManagerEnviroment.Settings;

            Dns = Settings.Dns;
            TargetEmail = Settings.TargetEmail;
            PrinterType = Settings.PrinterType;
            Purge = Settings.Purge;
            Threshold = Settings.Threshold;
            DefaultPrinter = Settings.DefaultPrinter;
            MaximumSpoolHistorie = Settings.MaximumSpoolHistorie;

            base.BuildCompled();
        }


        private static Func<string> GetMessage(string name) => () => UIResources.ResourceManager.GetString(name);

        private static bool ValidateDns(object value, ValidatorContext context)
        {
            string str = value as string;
            return !string.IsNullOrWhiteSpace(str) && IPAddress.TryParse(str, out var _);
        }
        private static bool ValidateEmail(object value, ValidatorContext context)
        {            
            string str = value as string;
            return string.IsNullOrWhiteSpace(str) || InternetAddress.TryParse(ParserOptions.Default, str, out _);
        }
        private static bool ValidateNumber(object value, ValidatorContext context)
        {
            int number = (int) value;

            return number >= 0;
        }

    }
}