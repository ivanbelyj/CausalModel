# CausalModel

A structure for representing possible states of a causal entity (such as plot, generalized character personality, aspects of natural language typological structure, etc.). Takes into account the probabilities and supports real-time fixation of a specific state (e.g., it can be useful in dynamic procedural generation of a logical set of facts in a game).

Previous deprecated project - https://github.com/ivanbelyj/CausalGeneration

## What is Causal model (CM)?

Data structure with related concepts developed for using in different projects, mainly in the tasks of simulation and procedural generation. It is called Causal model, but it takes inspiration from different things and most of all it resembles decision trees.
In fact, the Causal model is a directed acyclic graph, the nodes of which represent possible facts, events or properties of the entity being modelled, and the edges represent causal relationships.
The project is motivated primarily by the needs of the Raging Tomorrow and Jailpunk game projects, in particular, their procedural plot, characters generation and dialogs.

## Features

- A language-independent way of representing possible states of a causal entity
- Fixation - dynamic CM-based simulation integrating with external code
- Probabilities of causal relationships
- Logical grouping of factors included in fact cause
- Abstract facts and implementations with weights
- Modular architecture
- Blocks - convention-based nested causal models feature providing reusing, abstracting and polymorphism in causal models development
- Model instances functionality allowing differentiate resolved blocks created during model fixation
- Flexible blocks resolving control
- Actual probabilities estimation via Monte Carlo simulation for analyzing your causal models

# Core concepts

**Causal model** represents all possible situations or states. The process in which an individual situation or entity is determined is called **fixation**. The library supports dynamic fixation, which can be integrated with external code (for example, which is necessary to simulate plot responsive to players actions or dynamic personal changes).

## Fact

**Fact** (or causal model node) is the minimum element of the model. Regardless of whether it is a fact, property, event, or some other phenomenon that can be represented as part of a causal relationship, it is considered as a fact in terms of causal models. In the model, the minimum element that makes up the causal relationship and brings the consequence to life is the **factor**. As a rule, factors have their own probabilities and are grouped into **causes expressions** - a way of representing the complex logic of life in the model.
Although almost all facts have their causes, there are some **root causes** in the model that contain only factors without a cause.
At the fixation stage, the causes are evaluated starting from the root and marked as **occured** or **not occured** (for which the Fixator component is responsible). In the current implementation, causes support the most common logical operators - AND, OR and NOT.

## Abstract facts and variants

- A semantically abstract property or event. For example, the fact that a character is proficient in martial arts does not indicate which specific skill is actually present.
- Can be implemented by _one_ of its **variants**.
- If there are no occurred variants, the AF is considered **uncertain**
- From the implementations that have occured, one option is selected in accordance with the weights determined by the **weight factors** (another type of factors that can also be grouped)

## When to use CM: common recommendations

### ✅ Good to use

- Causal logic too huge to be represented in plain text. For example, it's not convenient to keep all the possible world history outcomes via conditional constuctions or objects in code
- Logic mostly isolated from external systems. For example, dialog structures are generally separated from the common game logic
- Logic that tends to be declarative

### May be used

- Probabilities estimation, simulations and risks assessment
- Expert systems operating with primitive boolean logic

### ❌ May be unpropriate

- Not very large causal logic with a low probability of growth that could be represented in code
- Logic that actively operates with a state or tends to be imperative. Causal model is acyclic and prefers declarative approaches such as recursion using blocks resolving
- Logic closely coupled with external components. For example, CM may be not suitable for shooter AI that relies heavily on navigating the world and using mathematical calculations

# Blocks

- **Block** is a black box, seamlessly integrated into the cause-and-effect relationships of the causal model.
- A block defines a **block convention** - a set of inputs and outputs, **block references**. Inputs refer to external facts for the block, and outputs become factors for facts that follow from the block.
- A potential implementation of a block is any causal model that satisfies its convention.
- A block does not know about the causal model that implement it.
- The fixation is accompanied by the **blocks resolving**, as a result of which the blocks are treated as actual sets of facts.

## Using blocks

- Every causal model has a clear set of dependencies, blocks, that it uses - **declared blocks**. Each of the declared blocks represents a semantically unique entity, has a unique (within the CM) name and also refers to some convention.
- Several declared blocks can use the same convention (for example, in a plot model, several characters may be used, whose personality is represented by a generalized model).

## Block Functions

- **Recursion**: Inspired by real life, where time is linear, CM has no concept of cycles and treats iterations as unique elements generating each other similar to recursion.
- **Abstraction**: A block can represent a logically unified, but actually complex part of a causal model. It hides details of the structure, visibility of which is not necessary for the overall model. For example, to model the history of a state, it is not necessary to know all the details of the cause-and-effect structure of one of its leaders (if they are not directly referenced from the overall model).
- **Reusability**: A complex entity can be used in the model multiple times without duplicating its structure. For example, if a character model is used multiple times in a plot model, it is advantageous to express the character model as a block.
- **Polymorphism and increasing resulting combinations**: A block's convention can be satisfied by multiple implementations. Fixating different implementations can lead to different results in the final application. Thus, external code can not only define the root causes of one general model by fixating the model accordingly to the situation, but also implement blocks with different cause-and-effect structures, potentially elevating the diversity of procedural generation.

# Monte Carlo probabilities estimation

There are separate assembly that can be used for running simulations and estimating probabilities. A simple algorithm has been implemented that runs a large number of simulations and calculates the average percentage of occurrence of each fact of the model. This can be useful in developing optimal procedural generation models or even in the field of expert systems or some types of research activities such as risk assessment.
