using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; }

    // This connects the publisher to the many games they have published
    public ICollection<GamePublisher> GamePublishers { get; set; }

    // Users who follow this publisher
    public ICollection<PublisherFollow> Followers { get; set; } = new List<PublisherFollow>();

}
