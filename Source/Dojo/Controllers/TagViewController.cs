using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using MonoTouch.Dialog;
using MonoTouch.MessageUI;
using MonoTouch.UIKit;

namespace Dojo
{
    public sealed class TagViewController : DialogViewController
    {
        private readonly HomeViewController _homeViewController;
        private readonly TagCache _tagCache = TagCache.Instance;
        private Section _appSection;
        private Section _dataSection;
        private MFMailComposeViewController _mailViewController;

        public TagViewController(HomeViewController homeViewController) : base(UITableViewStyle.Plain, null)
        {
            _homeViewController = homeViewController;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _dataSection = CreateTagSection();
            _appSection = CreateAppSection();
            Root = new RootElement(string.Empty)
            {
                _dataSection,
                _appSection
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            RefreshData();
        }

        private static Section CreateTagSection()
        {
            var result = new Section
            {
                HeaderView = new MenuSectionView("Tags")
            };
            return result;
        }

        private void ContactUs()
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                _mailViewController = new MFMailComposeViewController();
                _mailViewController.SetToRecipients(new[] { "smorenko@gmail.com" });
                _mailViewController.Finished += (sender, e) => e.Controller.DismissViewController(true, null);
                _mailViewController.SetSubject("Feedback");
                PresentViewController(_mailViewController, true, null);
            }
        }

        private Section CreateAppSection()
        {
            var result = new Section
            {
                HeaderView = new MenuSectionView("Settings")
            };
            result.Add(new MenuElement("About", Resources.AboutImage, () => new AboutAppViewController("About")));
            result.Add(new MenuElement("Feedback & Support", Resources.FlagImage, ()=> ContactUs()));
            return result;
        }

        private List<Element> CreateElements()
        {
            List<TagEntity> tags = _tagCache.GetAll();
            List<Element> result = tags.Select(x => new StringElement(x.Name, () => FilterImage(x)))
                                       .Cast<Element>()
                                       .ToList();
            return result;
        }

        private void FilterImage(TagEntity entity)
        {
            _homeViewController.FilterImages(entity);
            NavigationController.PushViewController(_homeViewController, true);
        }

        private void RefreshData()
        {
            _dataSection.Elements = CreateElements();
            Root.Reload(_dataSection, UITableViewRowAnimation.None);
        }
    }
}
