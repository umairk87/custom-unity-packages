# UK Framework - Component Assigner

A reusable Unity Editor tool for automatically finding components in a GameObject hierarchy and assigning them to an array or `List<T>` field on a target component.

## Features

- Assign components from a selected GameObject hierarchy
- Supports `GetComponentsInChildren`
- Supports arrays
- Supports `List<T>`
- Works with public and private fields
- Finds component types by name
- Marks the target component dirty after assignment
- Editor-only tool

## Installation

Install through Unity Package Manager using the Git URL:

https://github.com/umairk87/custom-unity-packages.git?path=/packages/com.uk.framework.componentassigner

## Usage

Open the tool from:

**Tools → Component Assigner**

### 1. Select Parent

Select the GameObject whose hierarchy should be searched.

### 2. Select Target Component

Select the component containing the field that should receive the component references.

### 3. Enter Component Type

Enter the component type name.

Example:

Rigidbody