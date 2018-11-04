# Nerdy Mishks Contributing Guide

Everyone SHALL have an equal opportunity to become a contributor.

## License And Terms

This contributing guide SHALL use the Apache 2.0 License

This code of conduct relies on the language of 
[Rfc2119](https://www.ietf.org/rfc/rfc2119.txt)

### Terms

- **CREATOR** - Any person that created or started the project.
- **ADMINISTRATOR** - Any person that manages organizational resources and system privileges
- **MAINTAINER** - Any person that enforces policy and process.
- **CONTRIBUTOR** - Any person that submits a patch to the source repository.
- **USER** - Any person that interacts with the final product and MAY contribute feedback.
- **AUTHOR** Any person that writes a specific piece of code or content.
- **PLATFORM** - The primary git repository that hosts the project. 
- **TICKET** - A new thread or item created in the project bug or issue tracker.
- **LICENSE** - The primary Open Source Licence that is applied to
  the source code repository.
  Unless otherwise stated, the LICENSE, refers to the
  [Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0) or
  higher license created by
  the [Apache Foundation](http://www.apache.org/).
- **PATCH** The set of changes and contributions that a 
  CONTRIBUTOR wants to apply to the project.

### Licensing

- The project LICENSE is the Apache 2.0 or greater license.
- The project MUST use the stated LICENSE. 
- The LICENSE must be placed in the LICENSE file in the root of 
  the git repository.
- A patch MUST use the same LICENSE as the project.
- A 3rd party library MUST use a software LICENSE that is compatible
  with the project's declared LICENSE.
  
## Credit and Ownership

- Code and content are owned by their AUTHORS.
- CONTRIBUTORS SHALL be responsible for crediting the AUTHOR.
- CONTRIBUTORS SHALL be responsible for adding their name to the  
  project contributor list.
- The copyrights of the code and content SHALL be collectively owned 
  by the AUTHORS of the code and content.

## PROCESS
 
- The project vision and roadmap MUST BE driven and managed by
  the  project CREATORS and ADMINISTRATORS
- The project vision and roadmap MAY BE influenced by MAINTAINERS
  CONTRIBUTORS, and USERS
- All change requests MUST BE submitted to the TICKET system
  specified by the project.
- All TICKETS MUST adhere to the **Submissions** section.
- A CONTRIBUTOR SHALL NOT directly commit to the project.
- A CONTRIBUTOR's work SHALL occur on a forked version of the project.
- A CONTRIBUTOR SHALL work on an existing issue or create an
  issue before requesting a merge.
- A CONTRIBUTOR SHALL use the PLATFORM's pull or merge request to submit a patch.
- A CONTRIBUTOR's patch SHALL adhere to the **Patch Submissions** section.
- Any person that has a PLATFORM account MAY dicuss the patch.
- A MAINTAINER MUST enforce the **Submissions** policy.
- A MAINTAINER MUST accept or reject A CONTRIBUTOR's patch.  
- A MAINTAINER MAY offer direction for a patch to be accepted after
  rejection.
- A CONTRIBUTOR MAY ask for direction to correct a rejected patch.
- MAINTAINERS that are not ADMINISTRATORS or CREATORS SHOULD allow another MAINTAINER
 to accept or reject their patch.
- A MAINTAINER that are not ADMINISTRATORS or CREATORS on a PROBRATION period 
 MUST allow another MAINTAINER to accept or reject their patch.
- A MAINTAINER can only be put on PROBATION by ADMINISTRATORS or CREATORS.
- MAINTAINERS MUST focus on merging acceptable patches from the community.
- The USER who created the ticket MUST close the ticket within 3 days of  aCLOSE REQUEST or 
 the request will be forcefully closed.  
- If an Administrator MUST close an open ticket that is FULFILLED, the USER MAY be suspended.
 from making future issues for not respecting the community's time. 


## Submissions

### Ticket Submissions
 - All ticket submissions are subject to the project's [CODE OF CONDUCT](CODE-OF-CONDUCT.md)
 - Any ticket that demands rather than asks for particular outcome MAY be closed without warning.
 
### Submitting a bug ticket
 - A USER MUST clearly describe the expected behavior and the unexpected issue.
 - A USER MUST provide the steps to reproduce the issue. 
 - A USER MAY provide a screenshot and extra information.
 - A bug ticket WITHOUT a clear description and steps to reproduce MUST BE closed.
 - Otherwise, a legitmate bug MUST stay open until solved or deemed WONTFIX.
 
### Submitting a feature request ticket
 - A USER MUST clearly describe the expected behavior or behaviors for the new feature.
 - A USER MUST clearly state the value of the new feature. 
 
### Submitting a question ticket
 - A USER MUST clearly ask a non-leading question.
 - A USER MUST ask a question specifically related to the project.  
 
### Patch Submissions
 - A patch MUST NOT violate the CODE OF CONDUCT, if defined.
 - A patch MUST NOT violate the CODING CONVENTIONS AND STANDARDS, if defined.
 - A patch MUST BE a patch or diff that git or the PLATFORM understands.
 - A patch MUST compile, pass tests, and execute on all supported platforms.
 - A patch MUST include the ticket identifier in the message.
 - A patch commit MUST include a short message of 50 characters.
 - A patch commit message MUST include the type of commit as the prefix. e.g. BUG, TASK, FEATURE, MERGE, PROTO, WIP, MAINT.

version 0.1.1