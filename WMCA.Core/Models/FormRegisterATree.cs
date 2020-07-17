using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Foolproof;

namespace FormRegisterATree.Models
{
    public class RegisterTreeFormViewModel
    {
        [Required]
        [Display(Name = "Are you registering as an individual or organisation?*")]
        public string WhoType { get; set; }

        [RequiredIf("WhoType", Operator.EqualTo, "Individual", ErrorMessage = "First Name is Required.")]
        [Display(Name = "First Name*:")]
        public string FirstName { get; set; }

        [RequiredIf("WhoType", Operator.EqualTo, "Individual", ErrorMessage = "Last Name is Required.")]
        [Display(Name = "Last Name*:")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address*:")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "How many trees did you plant?*")]
        public string TreesPlanted { get; set; }

        [RequiredIf("WhoType", Operator.EqualTo, "Organisation")]
        [Display(Name = "Organisation name*")]
        public string OrgName { get; set; }

        [Required]
        [Display(Name = "Date Planted:")]
        public string DatePlanted { get; set; }

        [Required]
        [Display(Name = "Location:")]
        public string Location { get; set; }

        [Display(Name = "Latitude:")]
        public string Latitude { get; set; }

        [Display(Name = "Longitude:")]
        public string Longitude { get; set; }

        [Display(Name = "Tree Type:")]
        public TreeTypes TreeType { get; set; }

        [Display(Name = "Upload image of your tree")]
        public string Files { get; set; }

        [Required]
        [Display(Name = "I agree to")]
        public bool Policy { get; set; }

        [Display(Name = "Join the mailing list to be kept up to date")]
        public bool MailingList { get; set; }

        internal T GetValue<T>(T v)
        {
            throw new NotImplementedException();
        }
    }
}

public enum TreeTypes
{
    Alder,
    Apple,
    Ash,
    Aspen,
    Beech,
    Birch,
    Blackthorn,
    Box,
    Buckthorn,
    Cedar,
    Cherry,
    Chestnut,
    Cypress,
    Dogwood,
    Elder,
    Elm,
    Eucalyptus,
    Larch,
    Fir,
    Guelder,
    Hawthorn,
    Hazel,
    Hemlock,
    Holly,
    Hornbeam,
    Horse,
    Juniper,
    Lime,
    Maple,
    Monkey,
    Oak,
    Pear,
    Pine,
    Plane,
    Plum,
    Poplar,
    Rowan,
    Spindle,
    Spruce,
    Sycamore,
    Walnut,
    Wayfaring,
    Whitebeam,
    Willow,
    Yew,
    Other
}