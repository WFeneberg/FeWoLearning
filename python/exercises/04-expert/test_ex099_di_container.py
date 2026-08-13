import pytest

from ex099_di_container import CircularDependencyError, Container, Lifetime


class Config:
    def __init__(self) -> None:
        self.value = "config"


class Repository:
    def __init__(self, config: Config) -> None:
        self.config = config


class Service:
    def __init__(self, repo: Repository) -> None:
        self.repo = repo


class CircularA:
    def __init__(self, b: "CircularB") -> None:
        self.b = b


class CircularB:
    def __init__(self, a: CircularA) -> None:
        self.a = a


def test_transient_creates_a_new_instance_every_resolve():
    container = Container()
    container.register(Config)

    assert container.resolve(Config) is not container.resolve(Config)


def test_singleton_returns_the_same_instance_every_time():
    container = Container()
    container.register(Config, lifetime=Lifetime.SINGLETON)

    first = container.resolve(Config)
    second = container.resolve(Config)

    assert first is second


def test_constructor_injection_resolves_the_whole_graph():
    container = Container()
    container.register(Config)
    container.register(Repository)
    container.register(Service)

    service = container.resolve(Service)

    assert isinstance(service, Service)
    assert isinstance(service.repo, Repository)
    assert isinstance(service.repo.config, Config)


def test_resolving_an_unregistered_interface_raises_key_error():
    container = Container()

    with pytest.raises(KeyError):
        container.resolve(Config)


def test_a_registered_factory_function_is_used_instead_of_the_class():
    container = Container()
    built = []

    def make_config() -> Config:
        config = Config()
        config.value = "from factory"
        built.append(config)
        return config

    container.register(Config, factory=make_config)

    resolved = container.resolve(Config)

    assert resolved.value == "from factory"
    assert resolved is built[0]


def test_circular_dependency_is_detected():
    container = Container()
    container.register(CircularA)
    container.register(CircularB)

    with pytest.raises(CircularDependencyError):
        container.resolve(CircularA)


def test_scoped_instances_are_shared_within_one_scope():
    container = Container()
    container.register(Config, lifetime=Lifetime.SCOPED)

    with container.scope():
        first = container.resolve(Config)
        second = container.resolve(Config)

    assert first is second


def test_scoped_instances_differ_across_scopes():
    container = Container()
    container.register(Config, lifetime=Lifetime.SCOPED)

    with container.scope():
        first = container.resolve(Config)

    with container.scope():
        second = container.resolve(Config)

    assert first is not second


def test_resolving_a_scoped_interface_outside_any_scope_raises():
    container = Container()
    container.register(Config, lifetime=Lifetime.SCOPED)

    with pytest.raises(RuntimeError):
        container.resolve(Config)


def test_singleton_lifetime_ignores_scope_boundaries():
    container = Container()
    container.register(Config, lifetime=Lifetime.SINGLETON)

    with container.scope():
        first = container.resolve(Config)

    with container.scope():
        second = container.resolve(Config)

    assert first is second


def test_a_previous_scope_resumes_after_a_nested_one_exits():
    container = Container()
    container.register(Config, lifetime=Lifetime.SCOPED)

    with container.scope():
        outer = container.resolve(Config)
        with container.scope():
            inner = container.resolve(Config)
            assert inner is not outer
        resumed = container.resolve(Config)
        assert resumed is outer
