using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Domain
{
    public static class ImageEntityExtensions
    {
        public static List<ImageEntity> ForAddOrUpdate(this List<ImageEntity> value)
        {
            return value.Where(x => x.Tags.IsNotEmpty()).ToList();
        }

        public static List<ImageEntity> ForRemove(this List<ImageEntity> value)
        {
            return value.Where(x => x.Tags.IsEmpty() && x.New == false).ToList();
        }
    }
}
