'use strict';
var gulp = require('gulp'),
    less = require('gulp-less'),
    cssmin = require('gulp-cssmin'),
    plumber = require('gulp-plumber'),
    rename = require('gulp-rename'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat'),
    prettify = require('gulp-html-prettify'),
    cleanhtml = require('gulp-cleanhtml'),
    image = require('gulp-image');

// Less Build Task
gulp.task('less', function() {
    gulp.src('./CFPThemeV1/less/styles.less')
        .pipe(plumber())
        .pipe(less())
        .pipe(gulp.dest('../CfpExchange/wwwroot/css'))
        .pipe(cssmin())
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest('../CfpExchange/wwwroot/css'));
});

//Javascript Task
gulp.task('scripts', function() {
    gulp.src('./CFPThemeV1/js/*.js')
        .pipe(concat('scripts.js'))
        .pipe(uglify())
        .pipe(gulp.dest('../CfpExchange/wwwroot/js'));
});

// Image Task
gulp.task('image', function() {
    gulp.src('./CFPExchange/img/*')
        .pipe(image({
            pngquant: true,
            optipng: false,
            zopflipng: false,
            jpegRecompress: false,
            jpegoptim: true,
            mozjpeg: true,
            guetzli: false,
            gifsicle: true,
            svgo: true,
            concurrent: 10
        }))
        .pipe(gulp.dest('../CfpExchange/wwwroot/images'));
});

gulp.task('watch', function() {
    gulp.watch('./CFPThemeV1/less/**/*.less', ['less']);
    gulp.watch('./CFPThemeV1/js/*.js', ['scripts']);
});

gulp.task('default', ['watch']);