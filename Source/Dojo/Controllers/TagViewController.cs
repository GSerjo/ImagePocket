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
        private readonly Section _dataSection;
        private readonly HomeViewController _homeViewController;
        private readonly TagCache _tagCache = TagCache.Instance;
        private MFMailComposeViewController _mailViewController;

        public TagViewController(HomeViewController homeViewController) : base(UITableViewStyle.Plain, null)
        {
            _homeViewController = homeViewController;
            List<TagEntity> tags = _tagCache.GetAll();
            _dataSection = CreateTagSection(tags);
            Root = new RootElement(string.Empty)
            {
                _dataSection,
                CreateAppSection()
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            _dataSection.Clear();
            _dataSection.AddAll(CreateElements(_tagCache.GetAll()));
            base.ViewWillAppear(false);
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
            result.Add(new MenuElement("Feedback & Support", Resources.FlagImage, () => ContactUs()));
            return result;
        }

        private List<Element> CreateElements(List<TagEntity> tags)
        {
            List<Element> result = tags.Select(x => new StyledStringElement(x.Name, () => FilterImage(x)))
                                       .Cast<Element>()
                                       .ToList();
            return result;
        }

        private Section CreateTagSection(List<TagEntity> tags)
        {
            var result = new Section
            {
                HeaderView = new MenuSectionView("Tags")
            };
            result.AddAll(CreateElements(tags));
            return result;
        }

        private void FilterImage(TagEntity entity)
        {
            _homeViewController.FilterImages(entity);
            NavigationController.PushViewController(_homeViewController, true);
        }
    }
}
