This folder is intentionally empty. Shared views are located in "src\CJG.Web.Shared\Views\" and copied
to here during the build process. This allows us to share the same views between the External and Internal
sites.

If you ever need to add views here that are NOT relevant to both projects, we will need to
revisit the way this is implemented. We would need to ensure a way of local Shared views and
imported Shared views co-existing safely.