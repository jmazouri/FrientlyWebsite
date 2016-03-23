/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        sass: {                              // Task
            dist: {                            // Target
                options: {                       // Target options
                    style: 'expanded'
                },
                files: {                         // Dictionary of files
                    'wwwroot/css/foundation-sites/foundation.css': 'bower_components/foundation-sites/assets/foundation.scss',
                    'wwwroot/css/font-awesome.css': 'bower_components/font-awesome/scss/font-awesome.scss',
                }
            }
        },
        copy: {
            main: {
                files:[
                  // includes files within path
                  { expand: true, flatten: true, src: ['bower_components/font-awesome/fonts/*'], dest: 'wwwroot/fonts/', filter: 'isFile' },
                  { expand: true, cwd: 'bower_components/tinymce/plugins/', src: ['**'], dest: 'wwwroot/js/tinymce/plugins/' },
                  { expand: true, flatten: true, src: 'bower_components/tinymce/themes/modern/theme.min.js', dest: 'wwwroot/js/tinymce/' },
                  { expand: true, flatten: true, src: 'bower_components/tinymce/tinymce.min.js', dest: 'wwwroot/js/tinymce/' },

                  { expand: true, flatten: true, src: ['bower_components/tinymce/skins/lightgray/fonts/*'], dest: 'wwwroot/js/tinymce/skins/lightgray/fonts/' },

                  { expand: true, flatten: true, src: 'bower_components/tinymce/skins/lightgray/skin.min.css', dest: 'wwwroot/js/tinymce/skins/lightgray/' },
                  { expand: true, flatten: true, src: 'bower_components/tinymce/skins/lightgray/content.min.css', dest: 'wwwroot/js/tinymce/skins/lightgray/' }
                ]
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-copy');
};