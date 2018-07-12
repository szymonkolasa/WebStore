/// <binding BeforeBuild='build-sass' />
var gulp = require('gulp');
var gulpSass = require('gulp-sass');

gulp.task('build-sass', function () {
    gulp.src('./wwwroot/css/*.scss')
        .pipe(gulpSass())
        .pipe(gulp.dest('./wwwroot/css'));
});