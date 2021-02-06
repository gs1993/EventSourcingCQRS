# Event Sourcing starter template

### Current framework version: .NET 5 

Based on: [https://github.com/VenomAV/EventSourcingCQRS](https://github.com/VenomAV/EventSourcingCQRS)

--------------

# Features
### Performance
* In a system under heavy load, event sourcing can is useful thanks to natural sharding and separating read and write models.

### Data modelling
* Event sourcing gives more flexibility, thanks to the separation of the action from effect. This approach grants flexibility in reacting to events happening in other 
parts of the system and creating multiple read models.

### Maintnance, bug fixing and auditing
* It's easy to reproduce state of aplication in given time, with helps reproducing bugs and audit trailing

--------------

## Setup

1. Download and install [Docker](https://www.docker.com/products/docker-desktop)
