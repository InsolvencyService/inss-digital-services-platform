# Resources

This is a temp location to simulate loading config for a page from some remote storage using the _TestStaticContentProvider_.

It provides a JSON file where the contents are a relative file path as some content is multiline and JSON doesn't support it.

Ultimately this content will reside in Azure Blob storage which is why paths are fully-qualified, for example:

```
/ip-upload/redundancy-payment/declaration/ipupload-declaration
```

where the last part is the unique key within the page (in this case declaration) for the value. This allows us to store
multiple values in blob storage for each page, if required and the paths will be driven from the URL the user is currently
on plus the key.

The blob structure is likely to be something like:

https://pagetest01.blob.core.windows.net/ip-upload/redundancy-payment/declaration/ipupload-declaration.html

where:

account is pagetest01
container is ip-upload
directories are redundancy-payment/declaration (which points at a specific page in the app)
resource is ipupload-declaration.html

There can be different designations (extensions) for differing contexts. For example a large piece of HTML that represents
a part or whole page can be stored in a .html file.

A piece of text such as a button may have a txt extension.

The idea will be to drive all the text via storage in blob with sensible caching locally so we can make changes without 
the need for rebuilding and redeployment.

**All of this may be** superseded by a configuration tool, although this may ultimately manage these resources in blob. 