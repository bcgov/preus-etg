/// <binding BeforeBuild='default' Clean='clean' ProjectOpened='watch' />
const ROOT_DIR = __dirname;
const SASS_DIR = ROOT_DIR + '/UI/scss';
const CSS_DIR = ROOT_DIR + '/css';
const JS_DIR = ROOT_DIR + '/js';

/**
 * Gulp configuration
 */
const gulp = require('gulp');
const gutil = require('gulp-util');
const postcss = require('gulp-postcss');
const cssnano = require('cssnano');
const assets = require('postcss-assets');
const sass = require('gulp-sass');
const del = require('del');
const webpack = require('webpack');
const webpackConfig = require('./UI/webpack.config.js');
const environments = require('gulp-environments');
const production = environments.production;
const development = environments.development;

const sassOptions = {
    includePaths: [
        require('bourbon').includePaths,
        require('bourbon-neat').includePaths
    ]
};


const assetsOptions = {
    loadPaths: [
        '../images/'
    ]
};

const processors = [
    assets(assetsOptions)
];

/**
 * Gulp Tasks
 */

gulp.task('clean', function () {
    return del([
        CSS_DIR + '/**/*',
        JS_DIR + '/**/*'
    ], { force: true });
});

gulp.task('sass', function () {
    if (production()) {
        // minify CSS
        processors.push(cssnano({
            // For IE compatibility
            svgo: false
        }));
    }
    return gulp.src(SASS_DIR + '/**/*.scss')
        .pipe(sass(sassOptions).on('error', sass.logError))
        .pipe(postcss(processors))
        .pipe(gulp.dest(CSS_DIR));
});


gulp.task('webpack', function (callback) {
    if (production()) {
        // minify JS
        webpackConfig.plugins.push(
            new webpack.optimize.UglifyJsPlugin({
                compress: {
                    warnings: false,
                    mangle: false
                }
            })
        );
    }
    webpack(webpackConfig, function (err, stats) {
        if (err) throw new gutil.PluginError('webpack', err);
        gutil.log('[webpack]', stats.toString({
            chunkModules: false
        }));
        callback();
    });
});

gulp.task('js:watch', function () {
    gulp.watch('./UI/javascript/**/*.js', gulp.parallel('webpack'));
});

gulp.task('sass:watch', function () {
    gulp.watch('./UI/scss/**/*.scss', gulp.parallel('sass'));
});

gulp.task('watch', gulp.parallel('sass:watch', 'js:watch'));

gulp.task('default', gulp.series('clean', 'webpack', 'sass'));
gulp.task('release', gulp.series('default'));
gulp.task('set-dev', gulp.series(development.task));
gulp.task('set-prod', gulp.series(production.task));

