# CausalModel

A structure for representing possible states of a causal entity (such as plot, generalized character personality, aspects of natural language typological structure, etc.). Takes into account the probabilities and supports real-time fixation of a specific state (e.g., it can be useful in dynamic procedural generation of a logical set of facts in a game).

Previous deprecated project - https://github.com/ivanbelyj/CausalGeneration

## What is Causal model ?
Data structure with related concepts developed for using in different projects, mainly in the tasks of simulation and procedural generation. It is called Causal model, but it takes inspiration from different things and most of all it resembles decision trees.
In fact, the Causal model is a directed acyclic graph, the nodes of which represent some facts, events or properties of the entity being modelled, and the edges represent causal relationships.

# Core concepts
**Causal model** represents all possible and correct situations or states. The process in which an individual situation or entity is determined is called **fixation**.
// Todo:

## Features
* Logical grouping of factors included in fact cause
* Probabilities of causal relationships
* Abstract facts, variants and implementation weights
* Flexible model fixation control, supporting using in runtime with external application systems
* Modular architecture
* Model JSON serialization / deserialization
* Blocks - convention-based nested causal models feature providing reusing, abstracting and polymorphism in causal models development
* Model instances functionality allowing differentiate resolved blocks created during model fixation
* Flexible blocks resolving control
* Actual probabilities estimation via Monte-Carlo simulation for analyzing your causal models


