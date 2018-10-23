using System;
using System.Collections.Generic;

namespace ClusterEngine
{
    public class EntityCollection
    {
        private int projectId;
        private List<Entity> entities;

        public EntityCollection()
        {
        }

        public void SetProjectId(int projectId)
        {
            this.projectId = projectId;
        }

        public void Add(Entity entity)
        {
            this.entities.Add(entity);

            EntityCollectionChangedEventArgs args = new EntityCollectionChangedEventArgs();
            args.ProjectId = this.projectId;
            args.Entities = this.entities;
            OnCollectionChanged(args);
        }

        public void AddRange(List<Entity> ents)
        {
            this.entities.AddRange(ents);

            EntityCollectionChangedEventArgs args = new EntityCollectionChangedEventArgs();
            args.ProjectId = this.projectId;
            args.Entities = this.entities;
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

        public event EntityCollectionChangedEventHandler CollectionChanged;
    }


    public class EntityCollectionChangedEventArgs : EventArgs
    {
        public int ProjectId { get; set; }
        public List<Entity> Entities { get; set; }
    }

    public delegate void EntityCollectionChangedEventHandler(Object sender, EntityCollectionChangedEventArgs e);

}
