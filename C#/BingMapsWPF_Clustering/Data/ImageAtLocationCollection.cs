using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClusterEngine;

namespace PhotoVis.Data
{
    public class ImageAtLocationCollection : IEnumerable<ImageAtLocation>
    {
        private int projectId;
        private List<ImageAtLocation> images = new List<ImageAtLocation>();

        public int Count
        {
            get
            {
                return images.Count;
            }
        }

        public ImageAtLocationCollection()
        {
        }

        public void SetProjectId(int projectId)
        {
            this.projectId = projectId;
        }

        public void Add(ImageAtLocation entity)
        {
            this.images.Add(entity);
            this.TriggerCollectionChanged();
        }

        public void AddRange(List<ImageAtLocation> ents)
        {
            this.images.AddRange(ents);
            this.TriggerCollectionChanged();
        }
        
        public List<Entity> AsEntities()
        {
            List<Entity> ents = new List<Entity>();
            foreach (var e in this.images)
            {
                ents.Add(e);
            }
            return ents;
        }

        public void Clear()
        {
            this.images.Clear();
        }

        public void TriggerCollectionChanged()
        {
            EntityCollectionChangedEventArgs args = new EntityCollectionChangedEventArgs();
            args.ProjectId = this.projectId;
            args.Entities = this.images;
            OnCollectionChanged(args);
        }

        protected virtual void OnCollectionChanged(EntityCollectionChangedEventArgs e)
        {
            EntityCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public ImageAtLocation this[int index]
        {
            get { return images[index]; }
            set { images.Insert(index, value); }
        }

        public IEnumerator<ImageAtLocation> GetEnumerator()
        {
            return images.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event EntityCollectionChangedEventHandler CollectionChanged;
    }

    public class EntityCollectionChangedEventArgs : EventArgs
    {
        public int ProjectId { get; set; }
        public List<ImageAtLocation> Entities { get; set; }
    }

    public delegate void EntityCollectionChangedEventHandler(Object sender, EntityCollectionChangedEventArgs e);
}
