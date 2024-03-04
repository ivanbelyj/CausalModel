# CausalModel

A structure for representing possible states of a causal entity (such as plot, generalized character personality, aspects of natural language typological structure, etc.). Takes into account the probabilities and supports real-time fixation of a specific state (e.g., it can be useful in dynamic procedural generation of a logical set of facts in a game).

Previous deprecated project - https://github.com/ivanbelyj/CausalGeneration

## What is Causal model ?
Data structure with related concepts developed for using in different projects, mainly in the tasks of simulation and procedural generation. It is called Causal model, but it takes inspiration from different things and most of all it resembles decision trees.
In fact, the Causal model is a directed acyclic graph, the nodes of which represent some facts, events or properties of the entity being modelled, and the edges represent causal relationships.
The project is motivated primarily by the needs of the Raging Tomorrow and Jailpunk game projects, in particular, their procedural plot and characters generation.

## Features
* Logical grouping of factors included in fact cause
* Probabilities of causal relationships
* Abstract facts, variants and implementation weights
* Dynamic fixation integrating with external code
* Modular architecture
* Model JSON serialization / deserialization
* Blocks - convention-based nested causal models feature providing reusing, abstracting and polymorphism in causal models development
* Model instances functionality allowing differentiate resolved blocks created during model fixation
* Flexible blocks resolving control
* Actual probabilities estimation via Monte-Carlo simulation for analyzing your causal models

# Core concepts
**Causal model** represents all possible and logically correct situations or states. The process in which an individual situation or entity is determined is called **fixation**. The library supports dynamic fixation, which can be integrated with external code (for example, which is necessary to stimulate plot responsive to players events or dynamic personal changes).

## Fact
**Fact** (or causal model node) is the minimum element of the model. Regardless of whether it is a fact, property, event, or some other phenomenon that can be represented as part of a causal relationship, it is considered as a fact in terms of causal models. As in real life, every fact has its causes, no matter how complex they may be. In the model, the minimum element that makes up the causal relationship and brings the consequence to life is the **factor**. As a rule, factors have their own probabilities and are grouped into **causes expressions** - a way of representing the complex logic of life in the model.
At the fixation stage, the causes are evaluated starting from the root and marked as **occured** or **not occured** (for which the Fixator component is responsible). In the current implementation, causes support the most common logical operators - AND, OR and NOT.

## Abstract facts
// Todo:





