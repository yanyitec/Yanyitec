using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    class AbsolutePath
    {
        const string RootRegExpression = "^[a-zA-Z]:(?:\\\\|/)[^/\\\\]?";
        static readonly System.Text.RegularExpressions.Regex RootPathRegx = new System.Text.RegularExpressions.Regex(RootRegExpression);
        public AbsolutePath (string path, StorageDirectory dir)
        {
            Orignal = path;
            if (RootPathRegx.IsMatch(path))
            {
                Absolute = path; IsOrignalAbsolute = true;this.Storage = Storaging.Storage.Root; return;
            }
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                if (dir.StorageType.IsStorage())
                {
                    Absolute = dir.FullName + path.TrimEnd('\\', '/');
                    this.Storage = dir as Storage;
                }
                else {
                    Absolute = dir.Storage + path.TrimEnd('\\', '/');
                    this.Storage = dir.InternalStorage;
                }
                IsOrignalAbsolute = false ;
                return;
            }
            if (dir.FullName == null) {
                Absolute = path;
                IsOrignalAbsolute = true;
                this.Storage = Storaging.Storage.Root;
                return;
            }

            Absolute = dir.FullName + "/" + path.Trim('\\', '/');
            IsOrignalAbsolute = false;
            this.Storage = dir.StorageType == StorageTypes.Storage ? dir as Storage : dir.InternalStorage;

        }

        public string Absolute { get;private set; }
        public string Orignal { get;private set; }
        public bool IsOrignalAbsolute { get; private set; }

        public Storage Storage {
            get; set;
        }

        public override string ToString()
        {
            return this.Absolute;
        }

        public static implicit operator string (AbsolutePath path) {
            return path.Absolute;
        }


    }
}
