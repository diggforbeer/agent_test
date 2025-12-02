# Issue Template System

This repository uses GitHub Issue Templates to organize work across different specialized agents.

## Templates Overview

### Main Template
- **Feature Request** (`feature-request.yml`) - Main template for requesting new features. Includes checkboxes to automatically assign work to specialized agents.

### Agent-Specific Templates
Each agent has a customized template tailored to their specific responsibilities:

1. **Product Agent Task** (`product-task.yml`)
   - Requirements and user story creation
   - Acceptance criteria definition
   - User persona and journey mapping
   - Business value assessment

2. **Developer Agent Task** (`developer-task.yml`)
   - Implementation and architecture
   - API design and database schema
   - Docker configuration
   - Code standards compliance

3. **Security Agent Task** (`security-task.yml`)
   - Authentication and authorization
   - Vulnerability fixes
   - OWASP compliance
   - Threat modeling

4. **Test Automation Agent Task** (`test-automation-task.yml`)
   - Unit, integration, and E2E tests
   - Test scenario definition
   - Code coverage goals
   - CI/CD test integration

5. **Code Review Agent Task** (`code-review-task.yml`)
   - Code quality assessment
   - Best practice recommendations
   - Refactoring suggestions
   - Performance and security review

## Workflow

### 1. Create a Feature Request
When you need a new feature:
1. Create a new issue using the "Feature Request" template
2. Fill in the feature description, acceptance criteria, and priority
3. Check the boxes for which agents should work on this feature
4. Submit the issue

### 2. Product Agent Creates Sub-Issues
When the **Product Agent** is assigned to work on a feature:
1. Review the parent feature request
2. Create detailed sub-issues for each checked agent using their specific templates:
   - Product task (if requirements need refinement)
   - Developer task (for implementation)
   - Security task (for security review)
   - Test automation task (for test coverage)
   - Code review task (for quality review)
3. Link each sub-issue to the parent using the "Parent Issue" field
4. Each sub-issue will have the template fields specific to that agent's work

### 3. Agents Work on Their Tasks
Each agent works on their assigned issues following the structure in their template:
- **Product Agent**: Defines requirements, user stories, and acceptance criteria
- **Developer Agent**: Implements features, designs APIs, configures Docker
- **Security Agent**: Reviews security, implements auth, fixes vulnerabilities
- **Test Automation Agent**: Writes comprehensive tests with good coverage
- **Code Review Agent**: Reviews code quality, suggests improvements

### 4. Track Progress
- Parent feature request remains open until all sub-tasks are complete
- Each agent updates their task with findings and implementations
- All agents reference the parent issue for context

## Example Workflow

```
Feature Request #1: User Authentication System
├── Product Task #2: Define authentication user stories
├── Developer Task #3: Implement ASP.NET Core Identity
├── Security Task #4: Security review of auth implementation
├── Test Task #5: Unit and integration tests for auth
└── Review Task #6: Code quality review of auth implementation
```

## Benefits

- **Clear Separation of Concerns**: Each agent has a focused, specialized template
- **Comprehensive Coverage**: Templates ensure all aspects are considered
- **Traceability**: Sub-issues link to parent features
- **Consistency**: Standardized fields across similar work
- **Quality**: Built-in checklists ensure nothing is missed

## Tips

- Always link sub-issues to the parent feature request
- Use the appropriate template for the type of work
- Fill in all required fields to provide complete context
- Update issues regularly with progress and findings
- Close sub-issues when complete; close parent when all sub-issues are done
