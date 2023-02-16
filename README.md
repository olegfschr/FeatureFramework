# FeatureFramework
Boilerplate framework which includes Features and Message System.

I use this framework for my recent projects to insure that game features are kept independent from each other ("don't know about each other") and communicate only trought the global message bus. This allows the features to be developed and tested even if other features it might depend on aren't developed yet.

Includes:
 - Feature class which concrete features can inherit from.
 - Message bus which allows to send and subscribe to messages (includes pool for caching messages and custom reference counter which should insure recycling of messages).
 - Bootstraper script which instantiates and initializes feature classes.
 - Simple example script which illustates usage of the framework.

The automatic instantiating of the feature classes is optinal, sometimes I do it by hand to have more control over features which are needed at the start of the game. 
