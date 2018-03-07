/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import * as React from 'react';
import {
  Platform,
  StyleSheet,
  Text,
  View,
  Button,
  StatusBar
} from 'react-native';

import UnityView from 'react-native-unity-view';

const instructions = Platform.select({
  ios: 'Press Cmd+R to reload,\n' +
    'Cmd+D or shake for dev menu',
  android: 'Double tap R on your keyboard to reload,\n' +
    'Shake or press menu button for dev menu',
});

type Props = {};

type State = {
  unity: boolean;
};

export default class App extends React.Component<Props, State> {
  
  constructor(props) {
    super(props);
    StatusBar.setHidden(false);
    StatusBar.setBarStyle('dark-content');
    this.state = {
      unity: false
    }
  }

  private onPress() {
    this.setState({ unity: !this.state.unity });
  }

  render() {
    return (
      <View style={[styles.container, { backgroundColor: this.state.unity ? "grey" : "red" }]}>
        {this.state.unity ? <UnityView style={{ position: 'absolute', left: 0, right: 0, top: 1, bottom: 1, }} /> : null}
        <Text style={styles.welcome}>
          Welcome to React Native!
        </Text>
        <Text style={styles.instructions}>
          To get started, edit App.js
        </Text>
        <Text style={styles.instructions}>
          {instructions}
        </Text>
        <Button title="Toggle Unity" color="blue" onPress={this.onPress.bind(this)} />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    // flex: 1,
    position: 'absolute', top: 0, bottom: 0, left: 0, right: 0,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF',
  },
  welcome: {
    fontSize: 20,
    textAlign: 'center',
    margin: 10,
  },
  instructions: {
    textAlign: 'center',
    color: '#333333',
    marginBottom: 5,
  },
});
